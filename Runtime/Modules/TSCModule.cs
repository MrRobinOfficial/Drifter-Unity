using Drifter.Components;
using Drifter.Vehicles;
using UnityEngine;

using static Drifter.Utility.DrifterMathUtility;

namespace Drifter.Modules
{
    [System.Serializable]
    public sealed class TSCModule : BaseECUModule
    {
        [SerializeField] Preset m_Preset = Preset.Custom;
        [SerializeField] float m_MinAngularVelocity = 1f;
        [SerializeField] float m_MaxAngularVelocity = Mathf.Infinity;
        [SerializeField, Range(0, 100)] float m_SlipThreshold = 12.5f;

        public override void Simulate(CarVehicle car, ECUComponent ecu, float deltaTime, ref float steerInput, ref float throttleInput, ref float brakeInput, ref float clutchInput, ref float handbrakeInput)
        {
            if (throttleInput <= THROTTLE_THRESHOLD)
            {
                IsEnabled = false;
                return;
            }

            var peakAngularVelocity = 0f;
            var peakSlipRatio = 0f;

            for (int i = 0; i < car.WheelArray.Length; i++)
            {
                var wheel = car.WheelArray[i];

                if (wheel.SlipRatio > peakSlipRatio)
                    peakSlipRatio = wheel.SlipRatio;

                if (wheel.AngularVelocity > peakAngularVelocity)
                    peakAngularVelocity = wheel.AngularVelocity;
            }

            if (peakAngularVelocity <= m_MinAngularVelocity ||
                peakAngularVelocity >= m_MaxAngularVelocity ||
                peakSlipRatio == 0f)
            {
                IsEnabled = false;
                return;
            }

            var gearRatio = car.Gearbox.CurrentGearRatio;
            peakSlipRatio *= Sign(gearRatio);

            if (peakSlipRatio > 0f && !IsEnabled)
                IsEnabled = true;

            if (peakSlipRatio < m_SlipThreshold && IsEnabled)
                IsEnabled = false;

            if (IsEnabled)
            {
                const float V = 10f;
                throttleInput = Mathf.Clamp(throttleInput - peakSlipRatio * V * (1f - clutchInput), 0f, 1f);
            }
        }
    }
}