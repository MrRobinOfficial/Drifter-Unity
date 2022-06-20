using Drifter.Misc;
using UnityEngine;

namespace Drifter.Extensions
{
    public static partial class DrifterExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AIR_DENSITY">In [kg/m^3]</param>
        /// <returns></returns>
        public static Vector3 CalcAerodynamics(this BaseVehicle vehicle)
        {
            var velocity = vehicle.transform.InverseTransformDirection(vehicle.Body.velocity);
            var dynamicPresure = 0.5f * WorldManager.Instance.AirDensity;
            var airSpeed = SqrVelocity(velocity.magnitude);

            var liftForce = vehicle.LiftCoefficient * dynamicPresure * airSpeed * vehicle.FrontalArea;

            var dragLongForce = -vehicle.DragCoefficient * dynamicPresure * airSpeed * vehicle.FrontalArea;

            //var dragLatForce = -(m_DragCoefficient * 2f) * dynamicPresure * airSpeed * (m_FrontalArea * 2f);

            return new Vector3(0f, liftForce, dragLongForce);

            static float SqrVelocity(float velocity) => velocity * Mathf.Abs(velocity);
        }
    }
}