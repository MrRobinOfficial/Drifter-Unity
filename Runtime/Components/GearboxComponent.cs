using System.Collections;
using Drifter.Extensions;
using UnityEngine.Events;
using UnityEngine;

using static Drifter.Extensions.DrifterExtensions;

namespace Drifter.Components
{
    [System.Serializable]
    public class GearboxComponent : BaseComponent
    {
        public event UnityAction OnShiftedUp;
        public event UnityAction OnShiftedDown;

        [field: Header("General Settings")]
        [field: SerializeField] public TransmissionType Type { get; set; } = TransmissionType.Manual;
        [field: SerializeField, Range(0f, 1f)] public float Efficiency { get; set; } = 0.9f;
        [field: SerializeField, Min(0f)] public float ShiftUpTime { get; set; } = 0.2f;
        [field: SerializeField, Min(0f)] public float ShiftDownTime { get; set; } = 0.15f;
        [field: SerializeField] public float ShiftUpRPM { get; set; } = 7500f;
        [field: SerializeField] public float ShiftDownRPM { get; set; } = 2000f;

        [field: Header("Gears")]
        [field: SerializeField] public float[] ForwardGears { get; set; } = new[] { 3.29f, 2.45f, 1.78f, 1.3f, 0.84f, };
        [field: SerializeField] public float ReverseGear { get; set; } = -2.5f;

        public void SetToNeutral() => _gearIndex = 0;

        public bool IsNeutral => _gearIndex == 0;

        private int _gearIndex;

        public int GearIndex
        {
            get => _gearIndex;
            set
            {
                if (ForwardGears == null)
                    return;

                var val = value;

                if (val < -1)
                    val = -1;

                if (val > ForwardGears.Length)
                    val = ForwardGears.Length;

                _gearIndex = val;
            }
        }

        public float GetOutputTorque(float inputTorque) => inputTorque * Efficiency * GetGearRatio(_gearIndex);

        public float GetInputShaftVelocity(float outputShaftVelocity) => outputShaftVelocity * Efficiency * GetGearRatio(_gearIndex);

        public float GetGearRatio(int index) => index switch
        {
            > 0 => ForwardGears[index - 1],
            < 0 => ReverseGear,
            _ => 0f,
        };

        public float CurrentGearRatio => GetGearRatio(_gearIndex);

        //public bool IsShifting => Time.timeAsDouble < _shiftTime;

        public bool IsShifting { get; private set; } = false;

        private double _shiftTime = 0d;

        private BaseVehicle vehicle;
        private Coroutine shiftCoroutine = null;

        public void ShiftUp()
        {
            if (shiftCoroutine != null)
                return;

            shiftCoroutine = vehicle.StartCoroutine(ShiftUpAsync());
        }

        public void ShiftDown()
        {
            if (shiftCoroutine != null)
                return;

            shiftCoroutine = vehicle.StartCoroutine(ShiftDownAsync());
        }

        public void ShiftToSpecificGear(int gearIndex)
        {
            if (IsShifting || GearIndex == gearIndex)
                return;

            GearIndex = gearIndex;
        }

        private IEnumerator ShiftUpAsync()
        {
            if (IsShifting || (GearIndex + 1 > ForwardGears.Length))
                yield break;

            _shiftTime = Time.timeAsDouble + ShiftUpTime;

            var lastIndex = GearIndex;
            GearIndex = 0;

            while (Time.timeAsDouble < _shiftTime)
            {
                IsShifting = true;
                yield return null;
            }

            IsShifting = false;
            GearIndex = lastIndex + 1;
            shiftCoroutine = null;

            lastGearShiftTime = Time.timeAsDouble;
        }

        private IEnumerator ShiftDownAsync()
        {
            if (IsShifting || (GearIndex - 1 < -1))
                yield break;

            _shiftTime = Time.timeAsDouble + ShiftDownTime;

            var lastIndex = GearIndex;
            GearIndex = 0;

            while (Time.timeAsDouble < _shiftTime)
            {
                IsShifting = true;
                yield return null;
            }

            IsShifting = false;
            GearIndex = lastIndex - 1;
            shiftCoroutine = null;

            lastGearShiftTime = Time.timeAsDouble;
        }

        public override void Init(BaseVehicle vehicle) => this.vehicle = vehicle;

        public struct GearboxData : IVehicleData
        {
            public EngineComponent engine;
            public ClutchComponent clutch;
            public float finalDriveRatio;
        } // 80 bytes of waste!!!

        private double lastGearShiftTime;

        public override void Simulate(float deltaTime, IVehicleData vehicleData = null)
        {
            var data = (GearboxData)vehicleData;

            if (Type != TransmissionType.Manual &&
                data.clutch.Type != ClutchType.FrictionDisc)
                AutomaticTransmission(data);

            void AutomaticTransmission(GearboxData data)
            {
                //const float SHIFT_UP_RPM = 7500f; // Scale and depends on DrivingMode. ECO lower RPM.
                //const float SHIFT_DOWN_RPM = 2000f;
                //const float SHIFT_REVERSE_RPM = 500f;
                //const float SHIFT_FIRST_RPM = 3500f;
                const float SPEED_THRESHOLD = 5f;
                const double SHIFT_TIME = 0.5d;

                var shiftTime = Time.timeAsDouble - lastGearShiftTime;
                var ableToShift = shiftTime > SHIFT_TIME;

                var speed = vehicle.GetSpeedInKph();
                var rpm = data.engine.GetRPM();

                if (ableToShift && rpm > ShiftUpRPM && (speed > SPEED_THRESHOLD || IsNeutral) &&
                    GearIndex >= 0)
                    ShiftUp();
                else if (ableToShift && rpm <= ShiftDownRPM && GearIndex > 1)
                    ShiftDown();

                //float GetShiftSpeed()
                //{
                //    var shiftUpSpeed = ShiftUpRPM.ToRads() / (GetGearRatio(GearIndex) * data.finalDriveRatio); // 14.35
                //    return shiftUpSpeed * 3.6f;
                //}

                //float GetTacho() => IsNeutral || data.clutch.IsEngaged ? data.engine.GetRPM() : data.clutch.GetRPM();
            }
        }

        #region Data Saving

        public override void LoadData(FileData data)
        {
            data.ReadValue("Gearbox", "Type", out TransmissionType type);
            data.ReadValue("Gearbox", "Efficiency", out float efficiency);
            data.ReadValue("Gearbox", "ShiftUpTime", out float shiftUpTime);
            data.ReadValue("Gearbox", "ShiftDownTime", out float shiftDownTime);

            Type = type;
            Efficiency = efficiency;
            ShiftUpTime = shiftUpTime;
            ShiftDownTime = shiftDownTime;

            //data.ReadValue("Gearbox Ratios", "ForwardGears", out float[] forwardGears);
            //data.ReadValue("Gearbox Ratios", "ReverseGears", out float reverseGear);

            //ForwardGears = forwardGears;
            //ReverseGear = reverseGear;
        }

        public override FileData SaveData()
        {
            var data = new FileData();

            data.WriteValue("Gearbox", "Type", Type);
            data.WriteValue("Gearbox", "Efficiency", Efficiency);
            data.WriteValue("Gearbox", "ShiftUpTime", ShiftUpTime);
            data.WriteValue("Gearbox", "ShiftDownTime", ShiftDownTime);

            var ratios = new FileData();
            ratios.WriteValue("Gearbox Ratios", "ForwardGears", ForwardGears);
            ratios.WriteValue("Gearbox Ratios", "ReverseGear", ReverseGear);

            data.Merge(ratios);

            return data;
        }

        public override void Shutdown() => throw new System.NotImplementedException();

        #endregion
    }
}