using System.Runtime.CompilerServices;
using UnityEngine;

namespace Drifter.Utility
{
    public class InterpolatedFloat
    {
        private float m_prevValue;
        private float m_value;

        public InterpolatedFloat(float initialValue = 0f) => Reset(initialValue);

        public void Reset(float value)
        {
            m_prevValue = value;
            m_value = value;
        }

        public void Set(float value)
        {
            m_prevValue = m_value;
            m_value = value;
        }

        public float Get() => m_value;

        public float GetInterpolated() => Mathf.Lerp(m_prevValue, m_value, MathUtility.GetFrameRatio());

        public float GetInterpolated(float t) => Mathf.Lerp(m_prevValue, m_value, t);
    }

    public class InterpolatedVector3
    {
        private Vector3 m_prevValue;
        private Vector3 m_value;

        public InterpolatedVector3(Vector3 initialValue = default(Vector3)) => Reset(initialValue);

        public void Reset(Vector3 value)
        {
            m_prevValue = value;
            m_value = value;
        }

        public void Set(Vector3 value)
        {
            m_prevValue = m_value;
            m_value = value;
        }

        public Vector3 Get() => m_value;

        public Vector3 GetInterpolated() => Vector3.Lerp(m_prevValue, m_value, MathUtility.GetFrameRatio());

        public Vector3 GetInterpolated(float t) => Vector3.Lerp(m_prevValue, m_value, t);
    }

    public static class MathUtility
    {
        public const float Kmh2Mph = 0.6213712f;
        public const float Nm2lbft = 0.7375621f;
        public const float Bar2PSI = 14.5038f;

        public static float Sinh(float x) => (Mathf.Exp(x) - Mathf.Exp(-x)) / 2f;

        public static float Cosh(float x) => (Mathf.Exp(x) + Mathf.Exp(-x)) / 2f;

        public static float Tanh(float x) => Sinh(x) / Cosh(x);

        public static float Sqr(float x) => x * x;

        public static float GetFrameRatio() =>
            Mathf.InverseLerp(Time.fixedTime, Time.fixedTime + Time.fixedDeltaTime, Time.time);

        public static Vector3 Rotated(this Vector3 vector, Quaternion rotation, Vector3 pivot = 
            default(Vector3)) => rotation * (vector - pivot) + pivot;

        public static Vector3 Rotated(this Vector3 vector, Vector3 rotation, Vector3 pivot = 
            default(Vector3)) => Rotated(vector, Quaternion.Euler(rotation), pivot);

        public static Vector3 Rotated(this Vector3 vector, float x, float y, float z, Vector3 pivot = default(Vector3)) => Rotated(vector, Quaternion.Euler(x, y, z), pivot);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRads(this float rpm) => rpm * 0.10472f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRPM(this float rads) => rads / 0.10472f;

        public static float UnclampedLerp(float from, float to, float t) => from + (to - from) * t;

        public static float Sign(float value, float tolerance = 0.01f) => value switch
        {
            > 1f => 1f,
            < -1f => -1f,
            _ => Approximately(value, 0f, tolerance) ? 0f : Sign(value),
        };

        public static bool Approximately(float a, float b, float tolerance = 0.01f) => 
            Mathf.Abs(a - b) <= tolerance;

        public static float Sign(float value) => value < 0f ? -1f : (value > 0f ? 1f : 0f);

        public static float Round(float value, int decimals = 2) => 
            (float)System.Math.Round(value, decimals, System.MidpointRounding.ToEven);

        public static float DistanceXZ(Vector3 a, Vector3 b)
        {
            float num = a.x - b.x;
            float num3 = a.z - b.z;
            return Mathf.Sqrt(num * num + num3 * num3);
        }

        public static bool NearlyEqual(float a, float b, float tolerance) => 
            Mathf.Abs(a - b) < tolerance;

        public static float SafeDivision(float a, float b) => (b == 0f) ? 0f : a / b;

        public static Vector3 SafeDivision(this Vector3 a, float b) => 
            new(SafeDivision(a.x, b), SafeDivision(a.y, b), SafeDivision(a.z, b));

        public static Vector3 SafeDivision(this Vector3 a, Vector3 b) => 
            new(SafeDivision(a.x, b.x), SafeDivision(a.y, b.y), SafeDivision(a.z, b.z));

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ConvertKphToMph(float kph)
        {
            const float V = 1.609f;
            return kph / V;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ConvertMsToKph(float meterPerSeconds)
        {
            const float V = 3.6f;
            return meterPerSeconds * V;
        }
    } 
}