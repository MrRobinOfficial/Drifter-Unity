using NaughtyAttributes;
using UnityEngine.Events;
using UnityEngine;

using Drifter.Modules;
using Drifter.Extensions;

using static Drifter.Extensions.DrifterExtensions;
using static Drifter.Utility.MathUtility;

namespace Drifter.Components
{
    [System.Serializable]
    public sealed class EngineComponent : BaseComponent
    {
        public event UnityAction OnStartup;
        public event UnityAction OnShutoff;

        /// <summary>
        /// 
        /// </summary>
        [field: Header("General Settings")]
        [field: SerializeField] public EngineType Type { get; set; } = EngineType.InternalCombustion;

        /// <summary>
        /// Efficiency of the engine. Ratio between 0 -> 1
        /// </summary>
        [field: SerializeField, Range(0f, 1f)] public float Efficiency { get; set; } = 0.8f;

        /// <summary>
        /// In [kg/m^2]
        /// </summary>
        [field: SerializeField, Tooltip("kg/m^2")] public float Inertia { get; set; } = 0.125f;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField, AllowNesting, ShowIf(nameof(Type), EngineType.InternalCombustion), Range(1, 12)] public byte Cylinders { get; set; } = 4;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField] public float FrictionTorque { get; set; } = 50f;

        /// <summary>
        /// Compression factor. Higher equals more compression of braking
        /// </summary>
        [field: SerializeField, Tooltip("Compression factor. Higher equals more compression of braking")] public float FrictionCoefficient { get; set; } = 0.02f;


        /// <summary>
        /// Idle RPM
        /// </summary>
        [field: Header("Torque & Power")]
        [field: SerializeField] public float IdleRPM { get; set; } = 900f;

        /// <summary>
        /// Maximum RPM (Red-line), is the point where engine start to break down.<br />
        /// The Engine can still operate, but it will damage in the process.<br />
        /// </summary>
        [field: SerializeField] public float MaxRPM { get; set; } = 8000f;

        /// <summary>
        /// Maximum power
        /// </summary>
        [field: SerializeField, Tooltip("Maximum power in [kW]")] public float MaxPower { get; set; } = 317f;

        /// <summary>
        /// RPM for giving the maximum power
        /// </summary>
        [field: SerializeField, Tooltip("RPM at maximum power")] public float PowerRPM { get; set; } = 5000f;

        [field: Header("Stalling")]
        [field: SerializeField] public bool EnableStalling { get; set; } = false;

        /// <summary>
        /// Stalls after reaching lower than this RPM
        /// </summary>
        [field: SerializeField, AllowNesting, ShowIf(nameof(EnableStalling)), Tooltip("Stalls after reaching lower than this RPM")] public float StallRPM { get; set; } = 900f;

        [field: Header("Thermal Settings")]
        [field: SerializeField] public bool EnableThermals { get; set; } = false;

        [field: Header("Starter")]
        [field: SerializeField] public bool AutoStart { get; set; } = true;
        [field: SerializeField] public float StarterTorque { get; set; } = 30f;

        /// <summary>
        /// In [seconds]
        /// </summary>
        [field: SerializeField, Tooltip("In [seconds]")]
        public float StarterDuration { get; set; } = 0.4f;

        [field: Header("Modules")]
        [field: SerializeField] public Optional<SuperchargerModule> Supercharger;
        [field: SerializeField] public Optional<TurbochargerModule> Turbocharger;
        [field: SerializeField] public Optional<RevlimiterModule> Revlimiter;
        [field: SerializeField] public Optional<LaunchControlModule> LaunchControl;
        [field: SerializeField] public Optional<NOSModule> Nitrous;

        // * Revlimter - Reduce fuel injection, so it doesn't red-line the engine

        /// <summary>
        /// In [rad/s]
        /// </summary>
        public float AngularVelocity { get; private set; } = 0f;

        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning { get; private set; } = false;

        public bool IsStalled { get; private set; } = false;

        public float GetRPM() => AngularVelocity.ToRPM();

        public float GetRPMNormalized(bool includeIdle) => 
            includeIdle ? (GetRPM() - IdleRPM) / (MaxRPM - IdleRPM) : GetRPM() / MaxRPM;

        /// <summary>
        /// Creates a curve based on points and engine speed <br/>
        /// <i>Formula from Vehicle Dynamics: Theory and Application by Reza N. Jazar.</i> 
        /// <a href="https://link.springer.com/book/10.1007/978-0-387-74244-1">
        /// <b>Link to his book</b></a><br/>
        /// </summary>
        /// <param name="angularSpeed">Engine speed in [rad/s]</param>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <param name="p3">Point 3</param>
        /// <returns></returns>
        private float EvaluateTorqueCurve(float angularSpeed, float p1, float p2, float p3)
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

        public float GetMaxEffectiveTorque(float RPM)
        {
            var angularSpeed = RPM.ToRads();
            var power = MaxPower * 1000f; // Kilowatts -> Watts
            var powerSpeed = PowerRPM.ToRads();

            var torque = EvaluateTorqueCurve
            (
                angularSpeed,
                p1: power / powerSpeed,
                p2: power / Mathf.Pow(powerSpeed, 2f),
                p3: -(power / Mathf.Pow(powerSpeed, 3f))
            ) * Sign(angularSpeed);

            return torque;
        }

        /// <summary>
        /// Gets power in [kW]
        /// </summary>
        /// <param name="torque"></param>
        /// <returns></returns>
        public float GetPowerFromTorque(float torque)
        {
            const float V = 9550f;
            return torque * AngularVelocity.ToRPM() / V;
        }

        /// <summary>
        /// Starts the engine
        /// </summary>
        public void Startup()
        {
            IsRunning = true;
            AngularVelocity = IdleRPM.ToRads();
            OnStartup?.Invoke();
        }

        /// <summary>
        /// Stops the engine
        /// </summary>
        public void Shutoff()
        {
            IsRunning = false;
            AngularVelocity = 0f;
            OnShutoff?.Invoke();
        }

        public override void OnEnable(BaseVehicle vehicle)
        {
            if (AutoStart)
                Startup();
        }

        public override void OnDisable(BaseVehicle vehicle) => Shutoff();

        public float LoadRatio { get; private set; }

        public void Simulate(float deltaTime, float loadTorque, float throttleInput)
        {
            throttleInput = Mathf.Max(throttleInput, b: 0.105f); // Idle air control actuator

            var rpm = GetRPM();

            if (Revlimiter.Enabled)
                Revlimiter.Value.Simulate(deltaTime, rpm, ref throttleInput);

            var fricTorque = CalcFrictionTorque(rpm);
            var initialTorque = (GetMaxEffectiveTorque(rpm) + fricTorque) * throttleInput;
            var effectiveTorque = initialTorque - fricTorque;

            if (LaunchControl.Enabled) { }

            if (Supercharger.Enabled) { }

            if (Turbocharger.Enabled)
            {
                Turbocharger.Value.Simulate(deltaTime, this, throttleInput);

                var extraTorque = Turbocharger.Value.GetOutputTorque(effectiveTorque);
                effectiveTorque += extraTorque;
            }

            LoadRatio = SafeDivision(effectiveTorque, GetMaxEffectiveTorque(rpm));

            if (float.IsNaN(LoadRatio) || float.IsInfinity(LoadRatio))
                LoadRatio = 0f;

            AngularVelocity += (effectiveTorque - loadTorque) * Efficiency / Inertia * deltaTime;

            //var rps = newRPM / 60f;
            //var domaintFreq = (float)(Cylinders / 2f) * rps;

            // * Idle air control actuator - Idling(Clamp at IdleRPM) is controlled by ECU or clamp throttle value by a number

            //if (AngularVelocity < IdleRPM.ToRads())
            //    AngularVelocity = IdleRPM.ToRads();

            if (AngularVelocity < 0f)
                Shutoff();

            if (AngularVelocity > MaxRPM.ToRads())
                TakeDamage(sender: null, damage: 10000);

            // Creates linear curve
            float CalcFrictionTorque(float rpm) => FrictionCoefficient * rpm + FrictionTorque; 
        }

        #region Data Saving
        public override void Load(FileData data) => throw new System.NotImplementedException();

        public override FileData Save() => throw new System.NotImplementedException(); 
        #endregion
    }
}