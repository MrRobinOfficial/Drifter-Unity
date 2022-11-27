using UnityEngine;
using UnityEngine.Events;

namespace Drifter.Vehicles
{
    [AddComponentMenu("Tools/Drifter/Vehicles/Truck [Vehicle]"), DisallowMultipleComponent]
    public class TruckVehicle : BaseVehicle
    {
        public event UnityAction OnConnectionEstablished;

        private IHookable _connection = null;

        public IHookable Connection
        {
            get => _connection;
            set
            {
                _connection = value;
                OnConnectionEstablished?.Invoke();
            }
        }
    }
}