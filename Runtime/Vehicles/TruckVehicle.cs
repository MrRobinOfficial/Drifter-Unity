using UnityEngine;
using UnityEngine.Events;

namespace Drifter.Vehicles
{
    [AddComponentMenu("Tools/Drifter/Vehicles/Truck [Vehicle]"), DisallowMultipleComponent]
    public class TruckVehicle : BaseVehicle, IDamageable
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

        protected override void OnInit() { }

        protected override void OnSimulate(float deltaTime) { }

        protected override void OnFixedSimulate(float deltaTime) { }

        protected override void OnShutdown() { }

        public void TakeDamage(Object sender, ushort damage)
        {
            throw new System.NotImplementedException();
        }
    }
}