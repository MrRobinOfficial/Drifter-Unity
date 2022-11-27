using Drifter.Modules.ECUModules;
using Drifter.Vehicles;
using NaughtyAttributes;
using UnityEngine;

using static Drifter.Extensions.EasingFunction;

namespace Drifter.Components
{
    [System.Serializable]
    public class ECUComponent : BaseComponent
    {
        [Header("Reverse Assists")]
        [SerializeField] bool m_EnableReverseAssists = true;
        [SerializeField] float m_MaxReverseSpeedInKph = 15f;

        [Header("Brake Assists")]
        [SerializeField] bool m_EnableBrakeAssists = true;
        [SerializeField, ShowIf(nameof(m_EnableBrakeAssists)), Range(0.125f, 0.925f)]
        float m_BrakeThreshold = 0.5f;
        [SerializeField, ShowIf(nameof(m_EnableBrakeAssists))] EaseType m_BrakeSmoothing = EaseType.Linear;

        [field: Header("Modules")]
        [field: SerializeField, Label("Hill Start Assist (HSA)")] public Optional<HSAModule> HSA { get; set; } = default;
        [field: SerializeField, Label("Anti-Lock Braking System (ABS)")] public Optional<ABSModule> ABS { get; set; } = default;
        [field: SerializeField, Label("Traction Control System (TCS/ASR)")] public Optional<TSCModule> TSC { get; set; } = default;
        [field: SerializeField, Label("Electronic Stability Control (ECS/ESP)")] public Optional<ESCModule> ESC { get; set; } = default;
        [field: SerializeField] public Optional<CruiseControlModule> CruiseControl { get; set; } = default;
        [field: SerializeField] public Optional<SpeedLimiterModule> SpeedLimiter { get; set; } = default;

        public float SteerInput { get; private set; }
        public float ThrottleInput { get; private set; }
        public float BrakeInput { get; private set; }
        public float ClutchInput { get; private set; }
        public float HandbrakeInput { get; private set; }

        private CarVehicle carVehicle = default;

        [HideInInspector] private float steerVelocity;
        [HideInInspector] private float throttleVelocity;
        [HideInInspector] private float brakeVelocity;
        [HideInInspector] private float clutchVelocity;
        [HideInInspector] private float handbrakeVelocity;

        private BaseVehicle vehicle = null;

        public override void OnEnable(BaseVehicle vehicle) => this.vehicle = vehicle;

        public override void OnDisable(BaseVehicle vehicle) => this.vehicle = null;

        public void Simulate(float deltaTime, ref float steerInput, ref float throttleInput, ref float brakeInput, ref float clutchInput, ref float handbrakeInput)
        {
            if (vehicle == null)
                return;

            var speed = vehicle.GetSpeedInKph();

            if (m_EnableReverseAssists && speed < -m_MaxReverseSpeedInKph)
                throttleInput = 0f;

            if (m_EnableBrakeAssists && brakeInput > m_BrakeThreshold)
            {
                var func = GetEasingFunction(m_BrakeSmoothing);
                brakeInput = func.Invoke(start: m_BrakeThreshold, end: 1f, brakeInput);
            }
        }

        //public override void Simulate(float deltaTime, IVehicleData data = null)
        //{
        //    //var steerInput = carVehicle.RawSteerInput;
        //    //var throttleInput = carVehicle.RawThrottleInput;
        //    //var brakeInput = carVehicle.RawBrakeInput;
        //    //var clutchInput = carVehicle.RawClutchInput;
        //    //var handbrakeInput = carVehicle.RawHandbrakeInput;

        //    //if (m_EnableBrakeAssists && brakeInput > m_BrakeThreshold)
        //    //{
        //    //    var func = GetEasingFunction(m_BrakeSmoothing);
        //    //    brakeInput = func.Invoke(start: m_BrakeThreshold, end: 1f, brakeInput);
        //    //}

        //    //if (ABS.IsNotNull)
        //    //    ABS.Value.Simulate(carVehicle, this, deltaTime, ref steerInput, ref throttleInput, ref brakeInput, ref clutchInput, ref handbrakeInput);

        //    //if (TSC.IsNotNull)
        //    //    TSC.Value.Simulate(carVehicle, this, deltaTime, ref steerInput, ref throttleInput, ref brakeInput, ref clutchInput, ref handbrakeInput);

        //    //if (ESC.IsNotNull)
        //    //    ESC.Value.Simulate(carVehicle, this, deltaTime, ref steerInput, ref throttleInput, ref brakeInput, ref clutchInput, ref handbrakeInput);

        //    //if (CruiseControl.IsNotNull)
        //    //    CruiseControl.Value.Simulate(carVehicle, this, deltaTime, ref steerInput, ref throttleInput, ref brakeInput, ref clutchInput, ref handbrakeInput);

        //    //if (SpeedLimiter.IsNotNull)
        //    //    SpeedLimiter.Value.Simulate(carVehicle, this, deltaTime, ref steerInput, ref throttleInput, ref brakeInput, ref clutchInput, ref handbrakeInput);

        //    //const float smoothTime = 0.15f;

        //    //SteerInput = Mathf.SmoothDamp(SteerInput, steerInput, ref steerVelocity, smoothTime);
        //    //ThrottleInput = Mathf.SmoothDamp(ThrottleInput, throttleInput, ref throttleVelocity, smoothTime);
        //    //BrakeInput = Mathf.SmoothDamp(BrakeInput, brakeInput, ref brakeVelocity, smoothTime);
        //    //ClutchInput = Mathf.SmoothDamp(ClutchInput, clutchInput, ref clutchVelocity, smoothTime);
        //    //HandbrakeInput = Mathf.SmoothDamp(HandbrakeInput, handbrakeInput, ref handbrakeVelocity, smoothTime);
        //}

        #region Data Saving
        public override void Load(FileData data) => throw new System.NotImplementedException();
        public override FileData Save() => throw new System.NotImplementedException();
        #endregion
    }
}