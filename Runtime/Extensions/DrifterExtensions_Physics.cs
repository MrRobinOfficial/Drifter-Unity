using Drifter.Misc;
using UnityEngine;

namespace Drifter.Extensions
{
    public static partial class DrifterExtensions
    {
        public static Vector3 GetSimpleDrag(float dragCoefficient, Vector3 velocity) => 
            -dragCoefficient * velocity * Mathf.Abs(velocity.magnitude);

        public static Vector3 GetAdvancedDrag(float dragCoefficient, Vector3 area, Vector3 velocity, float airDensity = 1.29f) => 0.5f * airDensity * dragCoefficient * area * 
            Mathf.Pow(velocity.magnitude, 2);

        public static Vector3 GetRollingResistance(float rollingCoefficient, Vector3 velocity) => 
            -rollingCoefficient * velocity;

        public static Vector3 ApplyForce(float mass, Vector3 gravity, float slopeAngle = 0f) => 
            mass * Mathf.Sin(slopeAngle) * gravity;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AIR_DENSITY">In [kg/m^3]</param>
        /// <returns></returns>
        public static Vector3 CalcAerodynamics(this BaseVehicle vehicle)
        {
            var velocity = vehicle.transform.InverseTransformDirection(vehicle.Rigidbody.velocity);
            var dynamicPresure = 0.5f * WorldManager.Instance.AirDensity;
            var airSpeed = SqrVelocity(velocity.magnitude);

            //var liftForce = vehicle.LiftCoefficient * dynamicPresure * airSpeed * vehicle.FrontalArea;

            //var dragLongForce = -vehicle.DragCoefficient * dynamicPresure * airSpeed * vehicle.FrontalArea;

            ////var dragLatForce = -(m_DragCoefficient * 2f) * dynamicPresure * airSpeed * (m_FrontalArea * 2f);

            return default;

            //return new Vector3(0f, liftForce, dragLongForce);

            static float SqrVelocity(float velocity) => velocity * Mathf.Abs(velocity);
        }
    }
}