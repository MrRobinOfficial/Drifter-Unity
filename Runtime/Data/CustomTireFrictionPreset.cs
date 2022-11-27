using UnityEngine;

using static Drifter.Utility.MathUtility;

namespace Drifter.Data
{
    [CreateAssetMenu(menuName = "Tools/Drifter/Presets/Tire Friction/Custom [Preset]", fileName = "New Custom Tire Friction Preset", order = 0)]
    public sealed class CustomTireFrictionPreset : BaseTireFrictionPreset
    {
        [SerializeField] AnimationCurve m_LongitudinalCurve = new(DefaultLongitudinalCurve.keys);
        [SerializeField] AnimationCurve m_LateralCurve = new(DefaultLateralCurve.keys);
        [SerializeField] AnimationCurve m_AligningCurve = new(DefaultAligningCurve.keys);


        private static readonly AnimationCurve DefaultLongitudinalCurve = AnimationCurve.EaseInOut(timeStart: 0f, valueStart: 1f, timeEnd: 1f, valueEnd: 0f);

        private static readonly AnimationCurve DefaultLateralCurve = AnimationCurve.EaseInOut(timeStart: 0f, valueStart: 1f, timeEnd: 1f, valueEnd: 0f);

        private static readonly AnimationCurve DefaultAligningCurve = AnimationCurve.EaseInOut(timeStart: 0f, valueStart: 1f, timeEnd: 1f, valueEnd: 0f);

        public override float CalcLongitudinalForce(float normalForce, float slipRatio) => normalForce * m_LongitudinalCurve.Evaluate(Mathf.Abs(slipRatio)) * Sign(slipRatio);

        public override float CalcLateralForce(float normalForce, float slipAngle, float camberAngle = 0) => normalForce * m_LateralCurve.Evaluate(Mathf.Abs(slipAngle)) * Sign(slipAngle);

        public override float CalcAligningTorque(float normalForce, float slipAngle, float camberAngle = 0) => normalForce * m_AligningCurve.Evaluate(Mathf.Abs(slipAngle)) * Sign(slipAngle);

        public override Vector3 GetCombinedForce(float normalForce, float slipRatio, float slipAngle, float camberAngle = 0)
        {
            var combinedSlip = new Vector2(slipRatio, slipAngle);
            var fNorm = combinedSlip.magnitude > 1f ? combinedSlip.normalized : combinedSlip;

            var longitudinalForce = CalcLongitudinalForce(normalForce, fNorm.x);
            var lateralForce = CalcLateralForce(normalForce, fNorm.y, camberAngle);
            var aligningTorque = CalcAligningTorque(normalForce, fNorm.y, camberAngle);

            return new Vector3(lateralForce, aligningTorque, longitudinalForce);
        }
    }
}