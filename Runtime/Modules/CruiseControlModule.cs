using Drifter.Components;
using Drifter.Vehicles;
using UnityEngine;

namespace Drifter.Modules
{
    [System.Serializable]
    public sealed class CruiseControlModule : BaseECUModule
    {
        //[SerializeField] int m_Increment = 5;
        [SerializeField, Range(30, 300)] float m_TargetSpeed = 30f;

        private bool isActivated = false;

        public void Activate()
        {
            isActivated = true;
            IsEnabled = false;
        }

        public void Deactivate()
        {
            isActivated = false;
            IsEnabled = false;
        }

        public void Toggle()
        {
            isActivated = !isActivated;
            IsEnabled = false;
        }

        public override void Simulate(CarVehicle car, ECUComponent ecu, float deltaTime, ref float steerInput, ref float throttleInput, ref float brakeInput, ref float clutchInput, ref float handbrakeInput)
        {
            if (!isActivated ||
                brakeInput > INPUT_THRESHOLD ||
                handbrakeInput > INPUT_THRESHOLD ||
                car.Gearbox.IsShifting)
            {
                isActivated = false;
                return;
            }

            var speed = car.Speedometer.Value;

            if (speed < m_TargetSpeed && !IsEnabled)
                IsEnabled = true;

            if (speed > m_TargetSpeed && IsEnabled)
                IsEnabled = false;

            if (IsEnabled)
                throttleInput = 1f;
        }
    }
}