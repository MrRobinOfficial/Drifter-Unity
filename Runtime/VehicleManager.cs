using System;
using System.Collections.Generic;

using static Drifter.Extensions.VehicleExtensions;

namespace Drifter
{
    public static partial class VehicleManager
    {
        public static IReadOnlyDictionary<Guid, BaseVehicle> Vehicles => _vehicles;

        public static int VehicleCount => _vehicles.Count;

        private static Dictionary<Guid, BaseVehicle> _vehicles = new();

        public static void AddVehicle(BaseVehicle vehicle)
        {
            if (!vehicle.TryGetGUID(out var guid))
                return;

            _vehicles.TryAdd(guid, vehicle);
        }

        public static void RemoveVehicle(BaseVehicle vehicle)
        {
            if (!vehicle.TryGetGUID(out var guid))
                return;

            _vehicles.Remove(guid);
        }

        public static bool TryGetVehicle<T>(string id, out T vehicle) where T : BaseVehicle
        {
            vehicle = default;

            if (!Guid.TryParse(id, out var guid))
                return false;

            BaseVehicle cacheVehicle;
            _vehicles.TryGetValue(guid, out cacheVehicle);

            vehicle = (T)cacheVehicle;
            return cacheVehicle != null;
        }

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        //private static void Init()
        //{
        //    OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        //    SceneManager.sceneLoaded += OnSceneLoaded;
        //}

        //private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        //{
        //    _vehicles.Clear();

        //    var vehicles = GameObject.FindObjectsOfType<BaseVehicle>(includeInactive: true);

        //    foreach (var veh in vehicles)
        //    {
        //        if (!veh.TryGetGUID(out var id))
        //            return;

        //        _vehicles[id] = veh;
        //    }
        //}

    }
}