using Drifter.Components;
using Drifter.Modules;
using Drifter.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using static Drifter.Extensions.DrifterExtensions;

namespace Drifter.Vehicles
{
    [AddComponentMenu("Tools/Drifter/Vehicles/Car [Vehicle]"), DisallowMultipleComponent]
    public sealed class CarVehicle : BaseVehicle
    {
        public event UnityAction OnBackfireEvent;

        private const string k_EngineFileName = "Engine.ini";
        private const string k_ClutchFileName = "Clutch.ini";
        private const string k_GearboxFileName = "Gearbox.ini";
        private const string k_FrontAxleFileName = "FrontAxle.ini";
        private const string k_RearAxleFileName = "RearAxle.ini";

        [field: Header("Measurements")]
        [field: SerializeField] public float WheelBase { get; private set; } = 2.425f;
        [field: SerializeField] public float FrontTrack { get; private set; } = 1.460f;
        [field: SerializeField] public float RearTrack { get; private set; } = 1.420f;
        [field: SerializeField] public float TurningCircle { get; private set; } = 10.2f;

        [field: Header("Steering Settings")]
        [field: SerializeField] public uint SteerRange { get; private set; } = 900;
        [field: SerializeField] public uint SteeRatio { get; private set; } = 10;
        [field: SerializeField] public bool LockOnStandby { get; private set; } = true;

        [field: Header("Powertrain Settings")]
        [field: SerializeField] public DriveType DriveType { get; set; } = DriveType.FWD;
        [field: SerializeField] public TransmissionType TransmissionType { get; set; } = default;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField, ShowIf(nameof(DriveType), DriveType.FourWD)]
        public float High4WDRatio { get; set; } = 1f;

        /// <summary>
        /// 
        /// </summary>

        [field: SerializeField, ShowIf(nameof(DriveType), DriveType.FourWD)]
        public float Low4WDRatio { get; set; } = 3f;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField, Range(1f, 0f), ShowIf(nameof(DriveType), DriveType.AWD)] 
        public float AWDBias { get; set; } = 0.5f;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField, Range(1f, 0f)]
        public float BrakeBias { get; set; } = 0.5f;

        /// <summary>
        /// Enable switching throttle and brake pedal automatically (Arcade Style).
        /// </summary>
        [field: SerializeField, ShowIf(nameof(TransmissionType), TransmissionType.Automatic)] 
        public bool AutoReverse { get; set; } = false;

        [field: Header("Powertrain Components")]
        [field: SerializeField] public Optional<BatteryComponent> Battery { get; private set; } = default;
        [field: SerializeField] public Optional<ECUComponent> ECU { get; private set; } = default;
        [field: SerializeField] public EngineComponent Engine { get; private set; } = default;
        [field: SerializeField] public ClutchComponent Clutch { get; private set; } = default;
        [field: SerializeField] public GearboxComponent Gearbox { get; private set; } = default;
        [field: SerializeField] public AxleComponent FrontAxle { get; private set; } = default;
        [field: SerializeField] public AxleComponent RearAxle { get; private set; } = default;

        [field: Header("Powertrain Modules")]
        [field: SerializeField] public Optional<FuelModule> Fuel { get; private set; } = default;

        public InterpolatedFloat Speedometer { get; private set; } = new();
        public InterpolatedFloat Tachometer { get; private set; } = new();
        public InterpolatedFloat Odometer { get; private set; } = new();

        /// <summary>
        /// In [rad/s]
        /// </summary>
        public float DriveShaftVelocity { get; private set; } = 0f;

        public WheelBehaviour[] WheelArray
        {
            get
            {
                if (_wheelArray == null || _wheelArray.Length == 0)
                {
                    _wheelArray = new WheelBehaviour[]
                    {
                        FrontAxle.LeftWheel,
                        FrontAxle.RightWheel,
                        RearAxle.LeftWheel,
                        RearAxle.RightWheel,
                    };
                }

                return _wheelArray;
            }
        }

        private WheelBehaviour[] _wheelArray = default;

        public override void FetchData()
        {
            base.FetchData();

            this.TryFetchData(k_EngineFileName, Engine);
            this.TryFetchData(k_ClutchFileName, Clutch);
            this.TryFetchData(k_GearboxFileName, Gearbox);
            this.TryFetchData(k_FrontAxleFileName, FrontAxle);
            this.TryFetchData(k_RearAxleFileName, RearAxle);
        }

        public override void PushData()
        {
            base.PushData();

            this.TryPushData(k_EngineFileName, Engine);
            this.TryPushData(k_ClutchFileName, Clutch);
            this.TryPushData(k_GearboxFileName, Gearbox);
            this.TryPushData(k_FrontAxleFileName, FrontAxle);
            this.TryPushData(k_RearAxleFileName, RearAxle);
        }

        protected override void Load(FileData data)
        {
            base.Load(data);
        }

        protected override FileData Save()
        {
            var data = new FileData(base.Save());

            data.WriteValue("Steering", "SteerRange", SteerRange);
            data.WriteValue("Steering", "SteeRatio", SteeRatio);
            data.WriteValue("Steering", "LockOnStandby", LockOnStandby);

            return data;
        }

        public float GetMaxSteerAngle() => SteerRange * 0.5f;

        public float GetSteerAngle() => GetMaxSteerAngle() * _steerInput;

        private float _steerInput, _throttleInput, _brakeInput, _clutchInput, _handbrakeInput;

        public float GetSteerInput() => _steerInput;
        public float GetThrottleInput() => _throttleInput;
        public float GetBrakeInput() => _brakeInput;
        public float GetClutchInput() => _clutchInput;
        public float GetHandbrakeInput() => _handbrakeInput;

        public void SetSteerInput(float value) => _steerInput = Mathf.Clamp(value, -1f, 1f);

        public void SetThrottleInput(float value) => _throttleInput = Mathf.Clamp01(value);

        public void SetBrakeInput(float value) => _brakeInput = Mathf.Clamp01(value);

        public void SetClutchInput(float value) => _clutchInput = Mathf.Clamp01(value);

        public void SetHandbrakeInput(float value) => _handbrakeInput = Mathf.Clamp01(value);

        protected override void OnEnable()
        {
            base.OnEnable();

            if (ECU.Enabled)
                ECU.Value.OnEnable(this);

            Engine.OnEnable(this);
            Clutch.OnEnable(this);
            Gearbox.OnEnable(this);
            FrontAxle.OnEnable(this);
            RearAxle.OnEnable(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (ECU.Enabled)
                ECU.Value.OnEnable(this);

            Engine.OnDisable(this);
            Clutch.OnDisable(this);
            Gearbox.OnDisable(this);
            FrontAxle.OnDisable(this);
            RearAxle.OnDisable(this);
        }

        private float asd;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            var deltaTime = Time.fixedDeltaTime;

            //Speedometer.Set(Mathf.Abs(GetSpeedInKph()));

            const float DRIVESHAFT_RADIUS = 0.0150876f; // In [m]

            Speedometer.Set(DriveShaftVelocity * DRIVESHAFT_RADIUS);
            Tachometer.Set(Engine.GetRPM());

            //Odometer.Set();

            if (ECU.Enabled)
                ECU.Value.Simulate(deltaTime, ref _steerInput, ref _throttleInput, 
                    ref _brakeInput, ref _clutchInput, ref _handbrakeInput);

            FrontAxle.ApplySteering(GetSteerAngle() / SteeRatio);

            FrontAxle.ApplyBrake(_brakeInput * BrakeBias);
            RearAxle.ApplyBrake(Mathf.Max(_brakeInput, _handbrakeInput) * (1f - BrakeBias));

            var clutchInputTorque = Clutch.Torque;
            var gearboxOutputTorque = Gearbox.GetOutputTorque(clutchInputTorque);

            switch (DriveType)
            {
                case DriveType.AWD: AWDSimulate(); break;
                case DriveType.FWD: FWDSimulate(); break;
                case DriveType.RWD: RWDSimulate(); break;
            }

            Engine.Simulate(deltaTime, Clutch.Torque, _throttleInput);
            Gearbox.Simulate(deltaTime, Engine, Clutch);

            // TODO: Backfire and exhaust stuff!

            void FWDSimulate()
            {
                FrontAxle.ApplyMotor(FrontAxle.GetOutputTorque(deltaTime, gearboxOutputTorque));
                DriveShaftVelocity = Gearbox.GetInputShaftVelocity(FrontAxle.GetInputShaftVelocity());

                Clutch.Simulate(_clutchInput, DriveShaftVelocity, Engine, 
                    Gearbox.GetGearRatio(Gearbox.GearIndex));
            }

            void RWDSimulate()
            {
                RearAxle.ApplyMotor(RearAxle.GetOutputTorque(deltaTime, gearboxOutputTorque));
                DriveShaftVelocity = Gearbox.GetInputShaftVelocity(RearAxle.GetInputShaftVelocity());

                Clutch.Simulate(_clutchInput, DriveShaftVelocity, Engine, 
                    Gearbox.GetGearRatio(Gearbox.GearIndex));
            }

            void AWDSimulate()
            {
                var frontBias = AWDBias;
                var rearBias = 1f - AWDBias;

                FrontAxle.ApplyMotor(FrontAxle.GetOutputTorque(deltaTime,
                    gearboxOutputTorque * frontBias));

                RearAxle.ApplyMotor(RearAxle.GetOutputTorque(deltaTime,
                    gearboxOutputTorque * rearBias));

                var awd_axleShaftVelocity = (FrontAxle.GetInputShaftVelocity() + RearAxle.GetInputShaftVelocity()) / 2f;

                DriveShaftVelocity = Gearbox.GetInputShaftVelocity(awd_axleShaftVelocity);

                Clutch.Simulate(_clutchInput, DriveShaftVelocity, Engine, 
                    Gearbox.GetGearRatio(Gearbox.GearIndex));
            }
        }
    }
}