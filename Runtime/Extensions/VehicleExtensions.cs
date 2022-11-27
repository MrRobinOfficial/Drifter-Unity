using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Drifter.Extensions
{
    public static class VehicleExtensions
    {
        public static void RegisterVehicle(this BaseVehicle vehicle) => 
            VehicleManager.AddVehicle(vehicle);

        public static void UnregisterVehicle(this BaseVehicle vehicle) => 
            VehicleManager.RemoveVehicle(vehicle);

        public static bool TryGetGUID(this BaseVehicle vehicle, out System.Guid guid) => System.Guid.TryParse(vehicle.ID, out guid);

#if UNITY_EDITOR
        [MenuItem("CONTEXT/BaseVehicle/Fetch Data", isValidateFunction: false, priority: 1000300)]
        private static void FetchData_Vehicle(MenuCommand command)
        {
            var ctx = command.context as BaseVehicle;

            if (ctx == null)
                return;

            ctx.FetchData();
        }

        [MenuItem("CONTEXT/BaseVehicle/Push Data", isValidateFunction: false, priority: 1000300)]
        private static void PushData_Vehicle(MenuCommand command)
        {
            var ctx = command.context as BaseVehicle;

            if (ctx == null)
                return;

            ctx.PushData();
        }
#endif
    }
}