using UnityEngine;

namespace Drifter.Modules.ECUModules
{
    /// <summary>
    /// Cruise Control - Automatically speeds up and slows down your car to keep a set following distance relative to the car ahead.
    /// </summary>
    [System.Serializable]
    public sealed class CruiseControlModule : BaseECUModule
    {
        private const ushort MIN_SPEED = 30;
        private const ushort MAX_SPEED = 300;

        [SerializeField, Range(MIN_SPEED, MAX_SPEED)] ushort m_TargetSpeed = 30;
        [field: SerializeField] public byte Increment { get; set; } = 5;
        [field: SerializeField, Range(0f, 1f)] public float BrakeThreshold = 0.3f;

        public bool IsActived { get; set; } = false;

        public ushort TargetSpeed
        {
            get => m_TargetSpeed;
            set => m_TargetSpeed = (ushort)Mathf.Clamp(value, MIN_SPEED, MAX_SPEED);
        }

        public void Increase() => TargetSpeed += Increment;

        public void Decrease() => TargetSpeed -= Increment;

        public void Simulate(float deltaTime, in BaseVehicle vehicle, 
            ref float throttleInput, float brakeInput)
        {
            if (!IsActived)
                return;

            if (brakeInput >= BrakeThreshold)
            {
                IsActived = false;
                return;
            }

            var speed = vehicle.GetSpeedInKph();

            if (speed < TargetSpeed)
                throttleInput = 1f - (speed / TargetSpeed);

            // TODO: Smoothing?
        }
    }
}