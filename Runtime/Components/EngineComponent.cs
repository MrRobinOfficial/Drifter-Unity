using NaughtyAttributes;
using UnityEngine.Events;
using UnityEngine;

using Drifter.Modules;

using static Drifter.Extensions.DrifterExtensions;
using static Drifter.Utility.DrifterMathUtility;

using Baracuda.Monitoring;
using Drifter.Extensions;

namespace Drifter.Components
{
    [System.Serializable]
    public sealed class EngineComponent : BaseComponent
    {
        public event UnityAction OnStarted;
        public event UnityAction OnStoped;

        [field: Header("General Settings")]
        [field: SerializeField] public EngineType Type { get; set; } = EngineType.InternalCombustion;
        [field: SerializeField, Range(0f, 1f)] public float Efficiency { get; set; } = 0.8f;
        [field: SerializeField, Tooltip("kg/m^2")] public float Inertia { get; set; } = 0.125f;
        [field: SerializeField] public float FrictionTorque { get; set; } = 50f;
        [field: SerializeField, Tooltip("Compression factor. Higher equals more compression of braking")] public float FrictionCoefficient { get; set; } = 0.02f;

        [field: Header("Torque & Power")]
        [field: SerializeField] public float IdleRPM { get; set; } = 900f;
        [field: SerializeField] public float MaxRPM { get; set; } = 8000f;
        [field: SerializeField, Tooltip("Maximum power in [kW]")] public float MaxPower { get; set; } = 317f;
        [field: SerializeField, Tooltip("RPM at maximum power")] public float PowerRPM { get; set; } = 5000f;

        [field: Header("Stalling")]
        [field: SerializeField] public bool EnableStalling { get; set; } = false;
        [field: SerializeField, AllowNesting, ShowIf(nameof(EnableStalling)), Tooltip("Stalls after reaching lower than this RPM")] public float StallRPM { get; set; } = 900f;

        [field: Header("Starter")]
        [field: SerializeField] public bool AutoStart { get; set; } = true;
        [field: SerializeField, Tooltip("In [sec]")] public float StarterDuration { get; set; } = 0.4f;

        [field: Header("Modules")]
        [field: SerializeField] public Optional<RevlimiterModule> Revlimiter { get; private set; }
        [field: SerializeField] public Optional<LaunchControlModule> LaunchControl { get; private set; }
        [field: SerializeField] public Optional<TurbochargerModule> Turbocharger { get; private set; }

        public float AngularVelocity { get; private set; } = 0f;
        public bool IsRunning { get; private set; } = false;

        public float GetLoadNormalized() => SafeDivision(loadTorque, this.GetCurrentTorque());

        [HideInInspector] private float loadTorque;
        [HideInInspector] private float throttleInput;
        [HideInInspector] private bool isStalling;
        [HideInInspector] private bool isClutchEngaged;
        [HideInInspector] private bool isGearboxNeutral;

        public void LoadVariables(float loadTorque, float throttleInput, bool isStalling, bool isClutchEngaged, bool isGearboxNeutral)
        {
            this.loadTorque = loadTorque;
            this.throttleInput = throttleInput;
            this.isStalling = isStalling;
            this.isClutchEngaged = isClutchEngaged;
            this.isGearboxNeutral = isGearboxNeutral;
        }

        public float GetRPM() => AngularVelocity.ToRPM();

        //public float GetRPMNormalized() => GetRPM() / MaxRPM;
        //public float GetRPMNormalized2() => (GetRPM() - IdleRPM) / (MaxRPM - IdleRPM);

        /// <summary>
        /// Creates a curve based on points and engine speed
        /// </summary>
        /// <param name="angularSpeed">Engine speed in [rad/s]</param>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <param name="p3">Point 3</param>
        /// <returns></returns>
        private float GetTorqueCurve(float angularSpeed, float p1, float p2, float p3)
        {
            // Engine Torque Formula
            // Te = Pe / We = P1 + P2*We + P3*We^2
            // Te = Torque Engine
            // Pe = Power Engine
            // We = Angular Velocity Engine
            // P1..P3 = Constants

            // Indirect injection Diesel:
            const float IJD_P1 = 0.6f;
            const float IJD_P2 = 1.4f;

            // Direct injection Diesel:
            const float DJD_P1 = 0.87f;
            const float DJD_P2 = 1.13f;

            switch (Type)
            {
                case EngineType.IndirectDiesel:
                    p1 *= IJD_P1;
                    p2 *= IJD_P2;
                    break;

                case EngineType.DirectDiesel:
                    p1 *= DJD_P1;
                    p2 *= DJD_P2;
                    break;
            }

            return p1 + p2 * angularSpeed + p3 * Mathf.Pow(angularSpeed, 2f);
        }

        /// <summary>
        /// Gets the torque from specific speed.
        /// </summary>
        /// <param name="angularSpeed">Engine speed in [rads/s]</param>
        /// <returns>In [N/m]</returns>
        public float GetTorque(float angularSpeed)
        {
            var power = MaxPower * 1000f; // Kilowatts -> Watts
            var powerSpeed = PowerRPM.ToRads();

            var torque = GetTorqueCurve
            (
                angularSpeed,
                p1: power / powerSpeed,
                p2: power / Mathf.Pow(powerSpeed, 2f),
                p3: -(power / Mathf.Pow(powerSpeed, 3f))
            ) * Sign(angularSpeed);

            if (float.IsNaN(torque) || float.IsInfinity(torque) || torque < float.Epsilon)
                torque = 0f;

            return torque;
        }

        /// <summary>
        /// Gets power in [kW]
        /// </summary>
        /// <param name="torque"></param>
        /// <returns></returns>
        public float GetPower(float torque) => torque * AngularVelocity.ToRPM() / 9550f;

        public void Startup()
        {
            IsRunning = true;
            AngularVelocity = 0f;
        }

        public void Stop()
        {
            IsRunning = false;
            AngularVelocity = 0f;
        }

        public override void Init(BaseVehicle vehicle)
        {
            if (AutoStart)
                Startup();

            var peakTorque = 0f;

            for (float angularVelo = 0f; angularVelo < MaxRPM.ToRads(); angularVelo++)
            {
                var torque = GetTorque(angularVelo);

                if (torque > peakTorque)
                    peakTorque = torque;
            }

            Print($"Peak Torque: {peakTorque:N0} [N/m]", PrintType.Log);
        }

        public override void Simulate(float deltaTime, IVehicleData data = null)
        {
            if (!IsRunning)
                return;

            var rpm = AngularVelocity.ToRPM();
            var fricTorque = FrictionCoefficient * rpm + FrictionTorque; // creates linear curve
            var maxEffectiveTorque = GetTorque(AngularVelocity);
            var initialTorque = (maxEffectiveTorque + fricTorque) * throttleInput;
            var effectiveTorque = initialTorque - fricTorque;

            var angularVelo = AngularVelocity + (effectiveTorque - loadTorque) *
                Efficiency / Inertia * deltaTime;

            var idleRPM = isClutchEngaged || isGearboxNeutral ? IdleRPM : 0f;

            AngularVelocity = Mathf.Clamp(angularVelo, idleRPM.ToRads(), MaxRPM.ToRads());

            if (EnableStalling &&
                GetRPM() < StallRPM &&
                IsRunning && isStalling)
                Stop(); // Stall the engine

            if (GetRPM() > IdleRPM)
            {
                var num = fricTorque;

                if (num < 0f)
                    num = 0f;

                if (Mathf.Abs(num) > Mathf.Abs(effectiveTorque))
                    IsBackfiring = Random.value > 0.995f;
            }

            if (IsBackfiring)
                backfireTimer += Time.deltaTime * 10f;

            if (backfireTimer > 1f)
            {
                IsBackfiring = false;
                backfireTimer = 0f;
            }
        }

        public bool IsBackfiring { get; private set; } = false;

        private float backfireTimer = 0f;

        public override void Shutdown() { }

        #region Data Saving

        public override void LoadData(FileData data)
        {
            data.ReadValue("Engine", "Type", out EngineType type);
            data.ReadValue("Engine", "Efficiency", out float efficiency);
            data.ReadValue("Engine", "Inertia", out float inertia);
            data.ReadValue("Engine", "FrictionTorque", out float frictionTorque);
            data.ReadValue("Engine", "FrictionCoefficient", out float frictionCoefficient);

            Type = type;
            Efficiency = efficiency;
            Inertia = inertia;
            FrictionTorque = frictionTorque;
            FrictionCoefficient = frictionCoefficient;

            data.ReadValue("Engine", "IdleRPM", out float idleRPM);
            data.ReadValue("Engine", "MaxPower", out float maxPower);
            data.ReadValue("Engine", "MaxRPM", out float maxRPM);

            IdleRPM = idleRPM;
            MaxPower = maxPower;
            MaxRPM = maxRPM;

            data.ReadValue("Engine", "EnableStalling", out bool enableStalling);
            EnableStalling = enableStalling;

            if (enableStalling)
            {
                data.ReadValue("Engine", "StallRPM", out float stallRPM);
                StallRPM = stallRPM;
            }

            data.ReadValue("Engine", "StartDuration", out float starterDuration);

            StarterDuration = starterDuration;
        }

        public override FileData SaveData()
        {
            var data = new FileData();

            data.WriteValue("Engine", "Type", Type);
            data.WriteValue("Engine", "Efficiency", Efficiency);
            data.WriteValue("Engine", "Inertia", Inertia);
            data.WriteValue("Engine", "FrictionTorque", FrictionTorque);
            data.WriteValue("Engine", "FrictionCoefficient", FrictionCoefficient);

            data.WriteValue("Engine", "IdleRPM", IdleRPM);
            data.WriteValue("Engine", "MaxPower", MaxPower);
            data.WriteValue("Engine", "MaxRPM", MaxRPM);

            data.WriteValue("Engine", "EnableStalling", EnableStalling);

            if (EnableStalling)
                data.WriteValue("Engine", "StallRPM", StallRPM);

            data.WriteValue("Engine", "StartDuration", StarterDuration);

            return data;
        }

        #endregion
    }
}