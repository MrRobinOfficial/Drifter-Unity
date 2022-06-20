using UnityEngine;

namespace Drifter.Data
{
    [CreateAssetMenu(menuName = "Tools/Drifter/Presets/Camera [Preset]", fileName = "New Camera Preset", order = 10)]
    public class CameraPreset : ScriptableObject
    {
        public int baseFOV = 50;

        public void CalcFOV(ref float camFOV, float deltaTime, BaseVehicle vehicle)
        {
            var targetFOV = baseFOV;
            camFOV = Mathf.Lerp(camFOV, targetFOV, deltaTime);
        }

        public (float amplitude, float frequency) CalcShake(float deltaTime, BaseVehicle vehicle)
        {
            var amplitude = 0f;
            var frequency = 0f;

            return (amplitude, frequency);
        }
    }
}