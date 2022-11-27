using UnityEngine;

namespace Drifter.Modules.ECUModules
{
    /// <summary>
    /// Speed Limiter - Automatically limits your car speed to a set target speed .
    /// </summary>
    [System.Serializable]
    public sealed class SpeedLimiterModule : BaseECUModule
    {
        private const ushort MIN_SPEED = 30;
        private const ushort MAX_SPEED = 300;

        [SerializeField, Range(MIN_SPEED, MAX_SPEED)] ushort m_TargetSpeed = 30;
        [field: SerializeField] public byte Increment { get; set; } = 5;

        public ushort TargetSpeed
        {
            get => m_TargetSpeed;
            set => m_TargetSpeed = (ushort)Mathf.Clamp(value, MIN_SPEED, MAX_SPEED);
        }

        public bool IsActived { get; set; } = false;

        public void Increase() => TargetSpeed += Increment;

        public void Decrease() => TargetSpeed -= Increment;

        public void Simulate(float deltaTime, in BaseVehicle vehicle, ref float throttleInput)
        {
            if (!IsActived)
                return;

            var speed = vehicle.GetSpeedInKph();
            throttleInput = speed < TargetSpeed ? 1f - (speed / TargetSpeed) : 0f;

            // TODO: Smoothing?
        }
    }
}