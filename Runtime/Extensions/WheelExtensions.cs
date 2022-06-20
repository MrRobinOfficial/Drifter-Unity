using System.Collections.Generic;
using Unity.Collections;
using Drifter.Components;
using Drifter.Data;
using UnityEngine;

using static Drifter.Utility.DrifterMathUtility;

namespace Drifter.Extensions
{
    public static class WheelExtensions
    {
        public struct RaySort : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance) * -1;
        }

        private static readonly RaySort raySort = new();

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

        public static Mesh GenerateWheelCollider(WheelBehaviour wheel)
        {
            var tran = wheel.transform;
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            var halfWidth = wheel.Width / 1.5f;
            var theta = 0.0f;
            var startAngleOffset = Mathf.PI / 18.0f;
            var x = wheel.Radius * 0.5f * Mathf.Cos(theta);
            var y = wheel.Radius * 0.5f * Mathf.Sin(theta);
            var pos = tran.InverseTransformPoint(tran.position + tran.up * y + tran.forward * x);
            var newPos = pos;

            var vertexIndex = 0;
            for (theta = startAngleOffset; theta <= Mathf.PI * 2 + startAngleOffset; theta += Mathf.PI / 12.0f)
            {
                if (theta <= Mathf.PI - startAngleOffset)
                {
                    x = wheel.Radius * 1.08f * Mathf.Cos(theta);
                    y = wheel.Radius * 1.08f * Mathf.Sin(theta);
                }
                else
                {
                    x = wheel.Radius * 0.06f * Mathf.Cos(theta);
                    y = wheel.Radius * 0.06f * Mathf.Sin(theta);
                }

                newPos = tran.InverseTransformPoint(tran.position + tran.up * y + tran.forward * x);

                // Left Side
                var p0 = pos - tran.InverseTransformDirection(tran.right) * halfWidth;
                var p1 = newPos - tran.InverseTransformDirection(tran.right) * halfWidth;

                // Right side
                var p2 = pos + tran.InverseTransformDirection(tran.right) * halfWidth;
                var p3 = newPos + tran.InverseTransformDirection(tran.right) * halfWidth;

                vertices.Add(p0);
                vertices.Add(p1);
                vertices.Add(p2);
                vertices.Add(p3);

                // Triangles (double sided)
                // 013
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 0);

                // 023
                triangles.Add(vertexIndex + 0);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);

                pos = newPos;
                vertexIndex += 4;
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            return mesh;
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

        public enum RaycastCastType : byte
        {
            Single,
            Multiple,
            Sphere,
        }

        public static bool WheelCast(this WheelBehaviour wheel, RaycastCastType type, Ray ray, out RaycastHit hit, LayerMask layerMask) => type switch
        {
            RaycastCastType.Single => CastSingle(wheel, ray, out hit, layerMask),
            RaycastCastType.Multiple => CastMultiple(wheel, ray, out hit, layerMask),
            _ => CastSingle(wheel, ray, out hit, layerMask),
        };

        private static bool CastSingle(WheelBehaviour wheel, Ray ray, out RaycastHit hit, LayerMask layerMask)
        {
            const float GROUND_PENETRATION = 0.01f;

            var rideHeight = wheel.Suspension != null ? wheel.Suspension.GetRideHeight() : 0f;
            var maxDistance = rideHeight + (wheel.Radius * 2f);

            var isGrounded = Physics.Raycast(ray, out hit, maxDistance, layerMask);

            if (isGrounded && hit.transform != null && hit.transform.IsChildOf(wheel.transform.root))
                isGrounded = false;

            if (isGrounded && hit.collider != null && hit.collider.isTrigger)
                isGrounded = false;

            return isGrounded;
        }

        private static bool CastMultiple(WheelBehaviour wheel, Ray ray, out RaycastHit hit, LayerMask layerMask)
        {
            const float MAX_ANGLE = 360f;
            const int MAX_RAYS = 36;
            const int MAX_RAYS_TOTAL = MAX_RAYS * 3;

            ray.origin = wheel.transform.position;
            var rideHeight = wheel.Suspension != null ? wheel.Suspension.GetRideHeight() : 0f;
            var maxDistance = rideHeight + wheel.Radius;

            var results = new NativeArray<RaycastHit>(MAX_RAYS_TOTAL, Allocator.TempJob);
            var commands = new NativeArray<RaycastCommand>(MAX_RAYS_TOTAL, Allocator.TempJob);

            for (int i = 0; i < MAX_RAYS; i++)
            {
                var rot = Quaternion.AngleAxis(i * (float)(MAX_ANGLE / MAX_RAYS), wheel.transform.right);
                var direction = rot * wheel.transform.forward;

                var leftOrigin = ray.origin - wheel.transform.right * wheel.Width * 0.5f;
                var rightOrigin = ray.origin + wheel.transform.right * wheel.Width * 0.5f;

                commands[i] = new RaycastCommand(ray.origin, direction, maxDistance, layerMask);
                commands[i + 1] = new RaycastCommand(leftOrigin, direction, maxDistance, layerMask);
                commands[i + 2] = new RaycastCommand(rightOrigin, direction, maxDistance, layerMask);

                Debug.DrawRay(ray.origin, direction * maxDistance, Color.green);
                //Debug.DrawRay(leftOrigin, direction * maxDistance, Color.red);
                //Debug.DrawRay(rightOrigin, direction * maxDistance, Color.blue);
            }

            var handle = RaycastCommand.ScheduleBatch(commands, results, minCommandsPerJob: MAX_RAYS_TOTAL);
            handle.Complete();

            var minDist = float.MaxValue;
            var index = 0;

            for (int i = 0; i < results.Length; i++)
            {
                //if (results[i].collider != null &&
                //    results[i].collider.isTrigger)
                //    continue;

                //if (results[i].transform != null && 
                //    results[i].transform.IsChildOf(wheel.transform.root))
                //    continue;

                if (results[i].distance > float.Epsilon &&
                    results[i].distance < minDist)
                {
                    index = i;
                    minDist = results[i].distance;
                }
            }

            hit = results[index];

            results.Dispose();
            commands.Dispose();

            return hit.collider != null;
        }

        /// <summary>
        /// Check if a hit point is inside the cylinder defined by center, radius and width
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="hitPoint"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static bool CastCylinder(this Transform transform, Vector3 hitPoint, Vector3 center, float radius, float width)
        {
            // Seems like the orientation is always on x axis
            var orientation = Vector3.right;
            var pointRel = transform.InverseTransformPoint(hitPoint);
            // center is now origin
            // dist on direction = pointRel scalar orientation
            var distRelDir = Vector3.Dot(pointRel, orientation) / orientation.magnitude;
            // Division shouldn't be necessary since orientation is normalized.
            // In case vector is outside the tire

            const float THRESHOLD = 2f;

            if (distRelDir > width / THRESHOLD || distRelDir < -width / THRESHOLD)
                return false;

            return true;
        }

        public static bool WheelCast(this WheelBehaviour wheel, float radius, float width, Vector3 position, Vector3 up, Vector3 forward,
            Vector3 right, out RaycastHit batchedHit, LayerMask layerMask)
        {
            const int MAX_RAYS = 25;

            var results = new NativeArray<RaycastHit>(MAX_RAYS, Allocator.TempJob);
            var commands = new Unity.Collections.NativeList<RaycastCommand>(MAX_RAYS, Allocator.TempJob);

            var halfWidth = width * 0.5f;
            var theta = 0f;
            var x = radius * Mathf.Cos(theta);
            var y = radius * Mathf.Sin(theta);
            var pos = position + up * y + forward * x;
            var newPos = pos;

            for (theta = 0f; theta <= Mathf.PI * 2f; theta += Mathf.PI / 12f)
            {
                x = radius * Mathf.Cos(theta);
                y = radius * Mathf.Sin(theta);

                newPos = position + up * y + forward * x;

                var startPos = pos - right * halfWidth;
                var endPos = newPos + right * halfWidth;
                var direction = endPos - startPos;

                commands.Add(new RaycastCommand(startPos, direction, distance: width, layerMask: layerMask));

                pos = newPos;
            }

            var handle = RaycastCommand.ScheduleBatch(commands, results, minCommandsPerJob: MAX_RAYS);
            handle.Complete();

            results.Sort(raySort);
            batchedHit = results[index: 0];

            results.Dispose();
            commands.Dispose();

            return batchedHit.distance > float.Epsilon;
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