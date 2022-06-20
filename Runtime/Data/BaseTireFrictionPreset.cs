using UnityEngine;

namespace Drifter.Data
{
	public abstract class BaseTireFrictionPreset : ScriptableObject
	{
		[field: SerializeField] public AnimationCurve GripCurve = new(DefaultGripCurve.keys);

		private static readonly AnimationCurve DefaultGripCurve = AnimationCurve.EaseInOut(timeStart: 0f, valueStart: 1.8f, timeEnd: 5000f, valueEnd: 1.2f);

		public abstract float CalcLongitudinalForce(float normalForce, float slipRatio);	
		public abstract float CalcLateralForce(float normalForce, float slipAngle, float camberAngle = 0f);	
		public abstract float CalcAligningTorque(float normalForce, float slipAngle, float camberAngle = 0f);
		public abstract Vector3 GetCombinedForce(float normalForce, float slipRatio, float slipAngle, float camberAngle = 0f);
    }
}