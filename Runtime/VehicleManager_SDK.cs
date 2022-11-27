using Drifter.Vehicles;
using UnityEngine;

namespace Drifter
{
    public static partial class VehicleManager
    {
        public static bool TryGetSpeedInKph(string id, out float speedInKph)
        {
            speedInKph = 0f;

            if (!TryGetVehicle(id, out CarVehicle car))
                return false;

            speedInKph = car.GetSpeedInKph();
            return true;
        }

        public static bool TryGetDisplayName(string id, out string displayName)
        {
            displayName = string.Empty;

            if (!TryGetVehicle(id, out BaseVehicle vehicle))
                return false;

            displayName = vehicle.DisplayName;
            return true;
        }
    }
}