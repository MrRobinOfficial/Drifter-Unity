using UnityEngine;

namespace Drifter.Modules.ECUModules
{
    /// <summary>
    /// HSA - (Hill Start Assist) helps prevent roll-back when starting up again from a stopped position on an incline.
    /// </summary>
    [System.Serializable]
    public sealed class HSAModule : BaseECUModule
    {
        [field: SerializeField] public float InclineAngle { get; set; } = 3f;
        [field: SerializeField, Range(0f, 1f)] public float ThrottleThreshold { get; set; } = 0.3f;

        public void Simulate(float deltaTime, in BaseVehicle vehicle, 
            float throttleInput, ref float brakeInput)
        {
            var sign = Vector3.Cross(vehicle.transform.up, Vector3.up).z < float.Epsilon ? -1 : 1;
            var angle = Vector3.Angle(vehicle.transform.up, Vector3.up);

            if (angle <= InclineAngle)
                return;

            var signedAngle = angle * sign;
            
            if (throttleInput >= ThrottleThreshold)
            {
                // Gradually removes brake pressure
            }

            // Should be a big delay when releasing brake pedal. Around ≈ 5 [sec]

            // Strength [Variable]

            // * Positive Angle = Up hill
            // * Negative Angle = Down hill
        }

        // TODO: Create curve function based on angle inclination
    }
}