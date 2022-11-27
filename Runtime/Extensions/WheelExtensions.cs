using System.Collections.Generic;
using Unity.Collections;
using Drifter.Components;
using Drifter.Data;
using UnityEngine;

using static Drifter.Utility.MathUtility;

namespace Drifter.Extensions
{
    public static class WheelExtensions
    {
        public struct RaySort : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance) * -1;
        }

        public static readonly RaySort RAY_SORTER = new();

        /// <summary>
        /// Gets Slip Ratio (Friction Torque Method)
        /// <br></br><i>Method from <see href="https://www.youtube.com/watch?v=tIZV5xHguAw">Ivan Novozhilov</see></i>
        /// </summary>
        /// <returns></returns>
        public static float GetSx(float deltaTime, WheelBehaviour wheel, float relaxationLength = 0.005f)
        {
            var targetAngularVelo = wheel.VelocityAtWheel.z / wheel.Radius;
            var targetAngularAccel = (wheel.AngularVelocity - targetAngularVelo) / deltaTime;
            var targetFrictionTorque = targetAngularAccel * wheel.GetInertia();
            var maxFrictionTorque = wheel.NormalForce * wheel.GetGripCoefficient() * wheel.Radius;
            var sxMax = wheel.IsLocked ? Sign(wheel.LongitudinalSlipVelocity) : Mathf.Clamp(SafeDivision(targetFrictionTorque, maxFrictionTorque), -100f, 100f);

            var damping = Mathf.Clamp(Mathf.Abs(wheel.LongitudinalSlipVelocity) / relaxationLength * deltaTime, 0f, 1f);

            var slip = (sxMax - wheel.SlipRatio) * damping;
            return wheel.SlipRatio + slip;
        }

        ///// <summary>
        ///// Gets Slip Angle
        ///// <br></br><i>Method from <see href="https://www.youtube.com/watch?v=ozL9KfYLAUg">Ivan Novozhilov</see></i>
        ///// </summary>
        ///// <returns></returns>
        //public static float GetSy(float deltaTime, ref float slipAngleDynamic, Vector3 velocityAtWheel, float relaxationLength = 0.01f, float angleThreshold = 8f)
        //{
        //    var lateralVelo = velocityAtWheel.x;
        //    var longitudinalVelo = velocityAtWheel.z;

        //    if (longitudinalVelo == 0f)
        //        return 0f;

        //    var damping = Mathf.Clamp(Mathf.Abs(lateralVelo) / relaxationLength * deltaTime, 0f, 1f);

        //    const float LOW_SPEED = 3f;
        //    const float HIGH_SPEED = 6f;

        //    var t = RemapClamped(velocityAtWheel.magnitude, LOW_SPEED, HIGH_SPEED, 0f, 1f);
        //    var slipPeak = angleThreshold * Sign(-lateralVelo);
        //    var slipAngle = Mathf.Lerp(slipPeak, GetSlipAngle(), t); // Lerp between high-speed and low-speed Slip Angle

        //    slipAngleDynamic += (slipAngle - slipAngleDynamic) * damping;
        //    slipAngleDynamic = Mathf.Clamp(slipAngleDynamic, -90f, 90f);

        //    float GetSlipAngle() => longitudinalVelo > 0f ? Mathf.Rad2Deg * (Mathf.Atan(-lateralVelo / Mathf.Abs(longitudinalVelo))) : 0f; // Also known as SA

        //    return slipAngleDynamic / angleThreshold;
        //}

        /// <summary>
        /// Gets camber angle (Method from <see href="https://www.youtube.com/watch?v=0jsENVOmkxc">GDC - Hamish Young</see>)
        /// </summary>
        /// <returns>In [deg]</returns>
        public static float GetCamberAngle(Vector3 normal, Vector3 up) => (Mathf.PI / 2f) - Mathf.Acos(Mathf.Clamp(Vector3.Dot(normal, up), -1f, 1f));

        /// <summary>
        /// Gets slip ratio (Method from <see href="https://www.youtube.com/watch?v=0jsENVOmkxc">GDC - Hamish Young</see>)
        /// </summary>
        /// <param name="longitudinalVelocity">In [m/s]</param>
        /// <param name="angularVelocity">In [rad/s]</param>
        /// <param name="Radius">In [m]</param>
        /// <returns>In [%]</returns>
        public static float GetSlipRatio(WheelBehaviour wheel)
        {
            // Spinning: SlipRatio=1
            // Rolling: SlipRatio=0
            // Locked: SlipRatio=-1

            // ISSUE, VERY SLOW TO BRAKE

            const float FULL_SLIP_VELOCITY = 4.0f;

            var longVelo = wheel.VelocityAtWheel.z;

            var slide = wheel.IsLocked ? Sign(longVelo) : Sign(wheel.AngularVelocity);
            var slipVelocity = ((wheel.AngularVelocity * wheel.Radius) - longVelo);

            var absRoadVelo = Mathf.Abs(longVelo);
            var damping = Mathf.Clamp(absRoadVelo / FULL_SLIP_VELOCITY, 0f, 1f);
            var slipRatioSAE = SafeDivision(slipVelocity, absRoadVelo) * damping;
            //var slipRatioSAE = SafeDivision(slipVelocity, absRoadVelo);

            return Mathf.Clamp(slipRatioSAE, -1f, float.PositiveInfinity);
        }

        /// <summary>
        /// Gets slip angle (Method from <see href="https://www.youtube.com/watch?v=0jsENVOmkxc">GDC - Hamish Young</see>)
        /// </summary>
        /// <param name="longitudinalVelocity">In [m/s]</param>
        /// <param name="lateralVelocity">In [m/s]</param>
        /// <returns>In [deg]</returns>
        public static float GetSlipAngle(Vector3 velocityAtWheel)
        {
            velocityAtWheel.y = 0f;

            const float FULL_ANGLE_VELOCITY = 2f;
            var damping = Mathf.Clamp(velocityAtWheel.magnitude / FULL_ANGLE_VELOCITY, 0f, 1f);

            if (velocityAtWheel.sqrMagnitude < float.Epsilon)
                return 0f;

            var sinSlip = velocityAtWheel.normalized.x;
            sinSlip = Mathf.Clamp(sinSlip, -1, 1); // To avoid precision errors.

            return -Mathf.Asin(sinSlip) * damping * damping * Mathf.Rad2Deg;
        }

        public static float GetSlipRatio2(float longitudinalVelocity, float angularVelocity, float radius)
        {
            const float fullSlipVelo = 4.0f;

            if (longitudinalVelocity <= float.Epsilon)
                return 0f;

            var absRoadVelo = Mathf.Abs(longitudinalVelocity);
            var damping = Mathf.Clamp(absRoadVelo / fullSlipVelo, 0f, 1f);

            var wheelTireVelo = angularVelocity * radius;
            return SafeDivision(wheelTireVelo - longitudinalVelocity, absRoadVelo) * damping;
        }

        public static float GetSlipAngle2(Vector3 velocityAtWheel)
        {
            const float FULL_ANGLE_VELOCITY = 2f;
            const float PI_2x = Mathf.PI * 2f;

            var damping = Mathf.Clamp(velocityAtWheel.magnitude / FULL_ANGLE_VELOCITY, 0f, 1f);

            return Mathf.Clamp(-Mathf.Atan2(velocityAtWheel.x, Mathf.Abs(velocityAtWheel.z)) * damping * damping, -PI_2x, PI_2x) * Mathf.Rad2Deg;
        }

        public static float CalcSuspension(this SuspensionPreset preset, WheelBehaviour wheel, float deltaTime, ref float compression, ref float lastLength, ref bool isFullCompressed, ref bool isFullUncompressed)
        {
            if (preset == null)
                return 0f;

            var isGrounded = wheel.IsGrounded;
            var currentLength = wheel.Hit.distance;
            var minLength = 0f;
            var maxLength = preset.GetRideHeight() + (wheel.Radius * 2f); // CalcMaxLength();

            if (isGrounded)
            {
                isFullCompressed = isFullUncompressed = false;

                if (currentLength < minLength)
                {
                    currentLength = minLength;
                    isFullCompressed = true;
                }
                else if (currentLength > maxLength)
                {
                    currentLength = maxLength;
                    isFullUncompressed = true;
                }
            }
            else
                currentLength = maxLength;

            compression = (maxLength - currentLength) / maxLength; // Normalized

            var damperForce = 0f;
            var springForce = wheel.IsGrounded ? preset.m_SpringStiffness * 1000f * preset.m_SpringCurve.Evaluate(compression) : 0f; // [N/mm] -> [N/m]

            if (isFullUncompressed)
            {
                // TODO: Avoid clipping
                //Debug.Log("AVOID CLIPPING!");
            }
            else if (isGrounded)
            {
                const float DAMPER_SCALE = 10f;

                var springVelocity = (currentLength - lastLength) / deltaTime;
                var absSpringVel = springVelocity < 0f ? -springVelocity : springVelocity;

                damperForce = springVelocity < 0f
                   ? preset.m_DamperBumpStiffness * preset.m_DamperBumpCurve.Evaluate(absSpringVel / DAMPER_SCALE)
                   : -preset.m_DamperReboundStiffness * preset.m_DamperReboundCurve.Evaluate(absSpringVel / DAMPER_SCALE);

                damperForce *= DAMPER_SCALE;
            }

            lastLength = currentLength;

            var force = isGrounded ? Mathf.Max(springForce + damperForce, 0f) : 0f;

            if (float.IsNaN(force) || float.IsInfinity(force))
                force = 0f;

            return force;
        }

        public static float GetSlipping(this WheelBehaviour wheel, float minRange = 0f, float maxRange = 30f)
        {
            const float SLIP_PEAK = 0.7f;

            var slip = new Vector2
            {
                x = wheel.VelocityAtWheel.x * SLIP_PEAK,
                y = wheel.LongitudinalSlipVelocity,
            }.magnitude;

            return RemapClamped(slip, minRange, maxRange, outA: 0f, outB: 1f);
        }

        public static bool IsLocked(this WheelBehaviour wheel, float minRange = 0f, float maxRange = 30f)
        {
            var slip = wheel.GetSlipping(minRange, maxRange);
            return slip >= 0.5f ? true : false;
        }

        public static float GetRPM(this WheelBehaviour wheel) => wheel.AngularVelocity.ToRPM();

        public static Vector2 GetCombinedSlip(this WheelBehaviour wheel)
        {
            var combinedSlip = new Vector2(wheel.SlipRatio, wheel.SlipAngle);
            return combinedSlip.magnitude > 1f ? combinedSlip.normalized : combinedSlip;
        }
    }
}