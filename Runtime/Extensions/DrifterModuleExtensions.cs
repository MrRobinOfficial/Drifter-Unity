using Drifter.Modules;
using UnityEngine;

namespace Drifter.Extensions
{
    public static partial class DrifterExtensions
    {
        public static void CalcFuel(this BaseVehicle vehicle, FuelModule fuel, float powerInkW)
        {
            const float densityOfFuel = 0.85f;

            const float c1 = 2.49f;
            const float c2 = 0.327f;
            const float c3 = 14.99f;
            const float c4 = 523.64f;
            const float c5 = 0.01f;

            const float r0 = 0.00991f;
            const float r1 = 0.0000195f;

            var distance = vehicle.Distance / 1000f; // [km]

            var consumption = c1 + c2 * powerInkW + c3 * vehicle.DragCoefficient * vehicle.FrontalArea + c4 * (r0 + 18f * r1) + c5 * vehicle.Mass; // [g/km]

            var rate = consumption * 0.001f / densityOfFuel * 100f;

            fuel.Volume -= distance / 100f * rate;
        }
    }
}