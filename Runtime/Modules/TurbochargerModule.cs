using Drifter.Components;
using UnityEngine;

namespace Drifter.Modules
{
    [System.Serializable]
    public sealed class TurbochargerModule : BaseModule
    {
        [field: Header("Settings")]
        [field: SerializeField] public float MaxBoost { get; set; } = 0.75f;
        [field: SerializeField] public float Inertia { get; set; } = 1f;
        [field: SerializeField] public float SpoolUpSpeed { get; set; } = 0.5f;
        [field: SerializeField] public float SpoolDownSpeed { get; set; } = 0.1f;
        [field: SerializeField] public float MaxRPM { get; set; } = 7200f;

        //public float RPM { get; private set; } = 0f;

        /// <summary>
        /// In [bar]
        /// </summary>
        public float Boost { get; private set; } = 0f;

        public bool IsSpooling { get; private set; } = false;

        private float spoolValue;

        public float GetBoostAsPSI()
        {
            const float V = 14.504f;
            return Boost * V;
        }

        public void Simulate(float deltaTime, in EngineComponent engine, float throttleInput)
        {
            if (throttleInput < spoolValue)
            {
                spoolValue -= deltaTime / SpoolDownSpeed;

                if (spoolValue < -Mathf.Abs(throttleInput))
                    spoolValue = -Mathf.Abs(throttleInput);

                IsSpooling = true;
            }
            else if (throttleInput > spoolValue)
            {
                spoolValue += deltaTime / SpoolUpSpeed;

                if (spoolValue > Mathf.Abs(throttleInput))
                    spoolValue = Mathf.Abs(throttleInput);

                IsSpooling = false;
            }

            if (spoolValue <= 0f)
            {
                IsSpooling = false;
                spoolValue = 0f;
            }

            spoolValue = Mathf.Clamp01(spoolValue);

            var rpmNormalized = Mathf.Clamp01(Mathf.Abs(engine.GetRPM()) / MaxRPM);

            const float k_Atm2Bar = 1.01325f;
            Boost = spoolValue * MaxBoost * Mathf.Sqrt(rpmNormalized) * k_Atm2Bar;
        }

        public float GetOutputTorque(float inputTorque) => inputTorque * Boost;

        // Backpressure - Backpressure from compressed air
        // Boost - Amount of boost pressure produced
        // Inertia - Turbine momentum; resistance to change in state
    }
}