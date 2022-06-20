using UnityEngine;

namespace Drifter.Utility
{
    public class InterpolatedFloat
    {
        public float Value
        {
            get => _value;
            set => _value = value;
        }

        public float GetInterpolated() => Mathf.Lerp(a: 0f, b: 0f, GetFrameRatio());

        public void Reset(float value) { }

        public static float GetFrameRatio() => Mathf.InverseLerp(Time.fixedTime, Time.fixedTime + Time.fixedDeltaTime, Time.time);

        private float _value;
        private float _prevValue;
    }

    public static class DrifterMathUtility
    {
        public static float UnclampedLerp(float from, float to, float t) => from + (to - from) * t;

        public static float Sign(float value, float tolerance = 0.01f) => value switch
        {
            > 1f => 1f,
            < -1f => -1f,
            _ => Approximately(value, 0f, tolerance) ? 0f : Sign(value),
        };

        public static bool Approximately(float a, float b, float tolerance = 0.01f) => Mathf.Abs(a - b) <= tolerance;

        public static float Sign(float value) => value < 0f ? -1f : (value > 0f ? 1f : 0f);

        public static float Round(float value, int decimals = 2) => (float)System.Math.Round(value, decimals, System.MidpointRounding.ToEven);

        public static float DistanceXZ(Vector3 a, Vector3 b)
        {
            float num = a.x - b.x;
            float num3 = a.z - b.z;
            return Mathf.Sqrt(num * num + num3 * num3);
        }

        public static bool NearlyEqual(float a, float b, float tolerance) => Mathf.Abs(a - b) < tolerance;

        public static float SafeDivision(float a, float b) => (b == 0f) ? 0f : a / b;

        public static Vector3 SafeDivision(this Vector3 a, float b) => new(SafeDivision(a.x, b), SafeDivision(a.y, b), SafeDivision(a.z, b));

        public static Vector3 SafeDivision(this Vector3 a, Vector3 b) => new(SafeDivision(a.x, b.x), SafeDivision(a.y, b.y), SafeDivision(a.z, b.z));

        public static float MapRangeUnclamped(float val, float inA, float inB, float outA, float outB) => Mathf.Lerp(outA, outB, Mathf.InverseLerp(inA, inB, val));

        public static float RemapClamped(float val, float inA, float inB, float outA, float outB)
        {
            var t = (val - inA) / (inB - inA);

            if (t > 1f)
                return outB;
            if (t < 0f)
                return outA;

            return outA + (outB - outA) * t;
        }

        public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
        {
            var c = current.eulerAngles;
            var t = target.eulerAngles;

            return Quaternion.Euler(
              Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
              Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
              Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
            );
        }

        public static bool Approximately(this Quaternion quatA, Quaternion value, float acceptableRange) => 1f - Mathf.Abs(Quaternion.Dot(quatA, value)) < acceptableRange;

        public static float ClampAngle(float angle, float minAngle, float maxAngle)
        {
            const float MAX_ANGLE = 360f;

            if (angle < -MAX_ANGLE)
                angle += MAX_ANGLE;

            if (angle > MAX_ANGLE)
                angle -= MAX_ANGLE;

            return Mathf.Clamp(angle, minAngle, maxAngle);
        }

        public static float ConvertKphToMph(float kph)
        {
            const float V = 1.609f;
            return kph / V;
        }
    } 
}