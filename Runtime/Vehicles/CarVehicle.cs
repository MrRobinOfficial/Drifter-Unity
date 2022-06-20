using NaughtyAttributes;
using Drifter.Components;
using Drifter.Modules;
using UnityEngine;
using Drifter.Attributes;
using Drifter.Utility;

namespace Drifter.Vehicles
{
    [AddComponentMenu("Tools/Drifter/Vehicles/Car [Vehicle]"), DisallowMultipleComponent]
    public sealed class CarVehicle : BaseVehicle
    {
        [field: Header("Measurements")]
        [field: SerializeField] public float WheelBase { get; private set; } = 2.425f;
        [field: SerializeField] public float FrontTrack { get; private set; } = 1.460f;
        [field: SerializeField] public float RearTrack { get; private set; } = 1.420f;
        [field: SerializeField] public float TurningCircle { get; private set; } = 10.2f;

        [Header("Steering Settings")]
        [SerializeField] uint m_SteerRange = 900;
        [SerializeField] uint m_SteeRatio = 10;
        [SerializeField] bool m_LockOnStandby = true;

        [field: Header("Powertrain Settings")]
        [field: SerializeField] public DriveType DriveType { get; set; } = DriveType.FWD;
        [field: SerializeField] public TransmissionType TransmissionType { get; set; } = default;
        [field: SerializeField, ShowIf(nameof(DriveType), DriveType.FourWD)] public float High4WDRatio { get; set; } = 1f;
        [field: SerializeField, ShowIf(nameof(DriveType), DriveType.FourWD)] public float Low4WDRatio { get; set; } = 3f;
        [field: SerializeField, Range(1f, 0f), ShowIf(nameof(DriveType), DriveType.AWD)] public float AWDBias { get; set; } = 0.5f;
        [field: SerializeField, Range(1f, 0f)] public float BrakeBias { get; set; } = 0.5f;
        [field: SerializeField, ShowIf(nameof(TransmissionType), ~TransmissionType.Manual)] public bool AutoReverse { get; set; } = false;

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

        public bool IsGrounded => FrontAxle.IsGrounded || RearAxle.IsGrounded;

        public float RawSteerInput
        {
            get => _steerInput;
            set
            {
                if (!Engine.IsRunning && m_LockOnStandby)
                    return;

                _steerInput = Mathf.Clamp(value, -1f, 1f);
            }
        }

        public float RawThrottleInput
        {
            get => _throttleInput;
            set => _throttleInput = Mathf.Clamp(value, 0f, 1f);
        }

        public float RawBrakeInput
        {
            get => _brakeInput;
            set => _brakeInput = Mathf.Clamp(value, 0f, 1f);
        }

        public float RawClutchInput
        {
            get => _clutchInput;
            set => _clutchInput = Mathf.Clamp(value, 0f, 1f);
        }

        public float RawHandbrakeInput
        {
            get => _handbrakeInput;
            set => _handbrakeInput = Mathf.Clamp(value, 0f, 1f);
        }

        public float SteerInput => ECU.IsNotNull ? ECU.Value.SteerInput : RawSteerInput;
        public float ThrottleInput => ECU.IsNotNull ? ECU.Value.ThrottleInput : RawThrottleInput;
        public float BrakeInput => ECU.IsNotNull ? ECU.Value.BrakeInput : RawBrakeInput;
        public float ClutchInput => ECU.IsNotNull ? ECU.Value.ClutchInput : RawClutchInput;
        public float HandbrakeInput => ECU.IsNotNull ? ECU.Value.HandbrakeInput : RawHandbrakeInput;

        [HideInInspector] private float _leftSteerAngle, _rightSteerAngle;
        [HideInInspector] private float _steerInput, _throttleInput, _brakeInput, _clutchInput, _handbrakeInput;

        public float GetSteerAngle() => GetMaxSteerAngle() * SteerInput;

        public float GetSteerAngleNormalized() => GetSteerAngle() / GetMaxSteerAngle();

        public float GetMaxSteerAngle() => (float)m_SteerRange / 2;

        public float GetDriveshaftSpeedInKph()
        {
            var velo = 0f;

            switch (DriveType)
            {
                case DriveType.FWD:
                    velo += FrontAxle.LeftWheel.AngularVelocity * FrontAxle.LeftWheel.Radius;
                    velo += FrontAxle.RightWheel.AngularVelocity * FrontAxle.RightWheel.Radius;
                    velo /= 2;
                    break;

                case DriveType.RWD:
                    velo += RearAxle.LeftWheel.AngularVelocity * RearAxle.LeftWheel.Radius;
                    velo += RearAxle.RightWheel.AngularVelocity * RearAxle.RightWheel.Radius;
                    velo /= 2;
                    break;

                case DriveType.AWD:
                    velo += FrontAxle.LeftWheel.AngularVelocity * FrontAxle.LeftWheel.Radius;
                    velo += FrontAxle.RightWheel.AngularVelocity * FrontAxle.RightWheel.Radius;
                    velo += RearAxle.LeftWheel.AngularVelocity * RearAxle.LeftWheel.Radius;
                    velo += RearAxle.RightWheel.AngularVelocity * RearAxle.RightWheel.Radius;
                    velo /= 4;
                    break;
            }

            return Mathf.Abs(velo) * 3.6f;
        }

        private (float left, float right) GetWheelAngle()
        {
            var input = GetSteerAngleNormalized();

            if (input > float.Epsilon)
            {
                _leftSteerAngle = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurningCircle + (RearTrack / 2f))) * input;

                _rightSteerAngle = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurningCircle - (RearTrack / 2f))) * input;
            }
            else if (input < -float.Epsilon)
            {
                _leftSteerAngle = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurningCircle - (RearTrack / 2f))) * input;

                _rightSteerAngle = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurningCircle + (RearTrack / 2f))) * input;
            }
            else
            {
                _leftSteerAngle = 0f;
                _rightSteerAngle = 0f;
            }

            return (_leftSteerAngle, _rightSteerAngle);
        }

        protected override void OnInit()
        {
            if (Fuel.IsNotNull)
                Fuel.Value.Volume = Fuel.Value.Capacity;

            if (ECU.IsNotNull)
                ECU.Value.Init(this);

            if (FrontAxle.LeftWheel == null)
            {
                enabled = false;
                throw new System.ArgumentNullException(nameof(FrontAxle.LeftWheel), "Reference is missing");
            }

            if (FrontAxle.RightWheel == null)
            {
                enabled = false;
                throw new System.ArgumentNullException(nameof(FrontAxle.RightWheel), "Reference is missing");
            }

            if (RearAxle.LeftWheel == null)
            {
                enabled = false;
                throw new System.ArgumentNullException(nameof(RearAxle.LeftWheel), "Reference is missing");
            }

            if (RearAxle.RightWheel == null)
            {
                enabled = false;
                throw new System.ArgumentNullException(nameof(RearAxle.RightWheel), "Reference is missing");
            }

            Engine.Init(this);
            Clutch.Init(this);
            Gearbox.Init(this);
            FrontAxle.Init(this);
            RearAxle.Init(this);
        }

        protected override void OnSimulate(float deltaTime)
        {
            if (ECU.IsNotNull)
                ECU.Value.Simulate(deltaTime);

            var rightVelocity = Vector3.Dot(transform.right, Body.velocity);
            var forwardVelocity = Vector3.Dot(transform.forward, Body.velocity);
            var linearVelocity = new Vector2(rightVelocity, forwardVelocity).magnitude;

            //var smoothDeltaTime = Time.smoothDeltaTime;
            //const float GAUGE_SPEED = 20f;

            //Tachometer = Mathf.Lerp(Tachometer, Mathf.Abs(Engine.GetRPM()), smoothDeltaTime * GAUGE_SPEED);
        }

        protected override void OnFixedSimulate(float deltaTime)
        {
            //if (Fuel.IsNotNull)
            //    this.CalcFuel(Fuel.Value, Engine.GetCurrentPower());

            FrontAxle.ApplySteering(GetWheelAngle());

            FrontAxle.ApplyBrake(BrakeInput * (float)BrakeBias);
            RearAxle.ApplyBrake(Mathf.Max(BrakeInput, HandbrakeInput) * (1f - BrakeBias));

            var clutchInputTorque = Clutch.Torque;
            var gearboxOutputTorque = Gearbox.GetOutputTorque(clutchInputTorque);
            var outputModifier = IsDriveable ? 1f : 0f;

            switch (DriveType)
            {
                case DriveType.AWD:

                    var frontBias = AWDBias;
                    var rearBias = 1f - AWDBias;

                    FrontAxle.ApplyMotor(FrontAxle.GetOutputTorque(deltaTime, gearboxOutputTorque * frontBias * outputModifier));

                    RearAxle.ApplyMotor(RearAxle.GetOutputTorque(deltaTime, gearboxOutputTorque * rearBias * outputModifier));

                    var awd_axleShaftVelocity = (FrontAxle.GetInputShaftVelocity() + RearAxle.GetInputShaftVelocity()) / 2f;

                    var awd_gearboxShaftVelocity = Gearbox.GetInputShaftVelocity(awd_axleShaftVelocity);

                    Clutch.Simulate(ClutchInput, awd_gearboxShaftVelocity, Engine, Gearbox.GetGearRatio(Gearbox.GearIndex));
                    break;

                case DriveType.FWD:
                    FrontAxle.ApplyMotor(FrontAxle.GetOutputTorque(deltaTime, gearboxOutputTorque * outputModifier));

                    var fwd_gearboxShaftVelocity = Gearbox.GetInputShaftVelocity(FrontAxle.GetInputShaftVelocity());

                    Clutch.Simulate(ClutchInput, fwd_gearboxShaftVelocity, Engine, Gearbox.GetGearRatio(Gearbox.GearIndex));
                    break;

                case DriveType.RWD:
                    RearAxle.ApplyMotor(RearAxle.GetOutputTorque(deltaTime, gearboxOutputTorque * outputModifier));

                    var rwd_gearboxShaftVelocity = Gearbox.GetInputShaftVelocity(RearAxle.GetInputShaftVelocity());

                    Clutch.Simulate(ClutchInput, rwd_gearboxShaftVelocity, Engine, Gearbox.GetGearRatio(Gearbox.GearIndex));
                    break;
            }

            var isStalling = !Gearbox.IsNeutral && !Clutch.IsEngaged;

            Engine.LoadVariables(Clutch.Torque, ThrottleInput, isStalling, Clutch.IsEngaged, Gearbox.IsNeutral);

            Engine.Simulate(deltaTime);
            Clutch.Simulate(deltaTime);

            var gearboxData = new GearboxComponent.GearboxData
            {
                engine = Engine,
                clutch = Clutch,
                finalDriveRatio = GetFinalDriveRatio(),
            };

            Gearbox.Simulate(deltaTime, gearboxData);
            FrontAxle.Simulate(deltaTime);
            RearAxle.Simulate(deltaTime);

            float GetFinalDriveRatio() => DriveType switch
            {
                DriveType.AWD => (FrontAxle.FinalDriveRatio + RearAxle.FinalDriveRatio) * 0.5f,
                DriveType.FWD => FrontAxle.FinalDriveRatio,
                DriveType.RWD => RearAxle.FinalDriveRatio,
                _ => 1f,
            };
        }

        protected override void OnShutdown() => ECU.Value.Shutdown();

        private void OnCollisionEnter(Collision collision)
        {
            var gForce = collision.relativeVelocity.magnitude / (Time.fixedDeltaTime * Mathf.Abs(Physics.gravity.y));

            const float G_FORCE_THRESHOLD = 1f;

            if (Mathf.Abs(gForce) <= G_FORCE_THRESHOLD)
                return;

            const float AIRBAG_THRESHOLD = 20f;

            //if (Mathf.Abs(gForce) > AIRBAG_THRESHOLD)
            //    OnAirbagTrigged?.Invoke();
        }

        #region Data Saving

        public override void LoadData(FileData data)
        {
            base.LoadData(data);

            data.ReadValue("Steering", "LockOnStandby", out m_LockOnStandby);
            data.ReadValue("Powertrain", "Type", out DriveType type);

            DriveType = type;

            if (DriveType == DriveType.AWD)
            {
                data.ReadValue("Powertrain", "AWDBias", out float awdBias);
                AWDBias = awdBias;
            }

            Engine.LoadData(data);
            Clutch.LoadData(data);
            Gearbox.LoadData(data);

            var frontAxleData = new FileData();
            frontAxleData["Differential"].Merge(data["Front Axle"]);
            FrontAxle.LoadData(frontAxleData);

            var rearAxleData = new FileData();
            rearAxleData["Differential"].Merge(data["Rear Axle"]);
            RearAxle.LoadData(rearAxleData);
        }

        public override FileData SaveData()
        {
            var data = base.SaveData();

            data.WriteValue("Steering", "LockOnStandby", m_LockOnStandby);
            data.WriteValue("Powertrain", "Type", DriveType);

            if (DriveType == DriveType.AWD)
                data.WriteValue("Powertrain", "AWDBias", AWDBias);

            data.Merge(Engine.SaveData());
            data.Merge(Clutch.SaveData());
            data.Merge(Gearbox.SaveData());

            var axles = new FileData();

            axles["Front Axle"].Merge(FrontAxle.SaveData()["Differential"]);
            axles["Rear Axle"].Merge(RearAxle.SaveData()["Differential"]);

            data.Merge(axles);

            return data;
        } 

        #endregion
    }
}