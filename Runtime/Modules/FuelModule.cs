using UnityEngine;
using UnityEngine.Events;

namespace Drifter.Modules
{
    [System.Serializable]
    public class FuelModule : BaseModule
    {
        public event UnityAction OnEmptyEvent;

        private const float EMPTY_FUEL_THRESHOLD = 0.1f;
        private const float FULL_FUEL_THRESHOLD = 0.95f;

        [field: SerializeField, Tooltip("In liters")] public float Capacity { get; set; } = 30f;
        [field: SerializeField] public float Rate { get; set; } = 9f;

        public bool IsEmpty => GetVolumeNormalized() <= EMPTY_FUEL_THRESHOLD;

        public bool IsFull => GetVolumeNormalized() >= FULL_FUEL_THRESHOLD;

        public float GetVolumeNormalized() => Volume / Capacity;

        public float Volume
        {
            get => _volume;
            set
            {
                if (_volume <= 0f)
                    OnEmptyEvent?.Invoke();

                _volume = Mathf.Clamp(value, 0f, Capacity);
            }
        }

        private float _volume;

        //public override void Init(BaseVehicle vehicle)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public override void Simulate(float deltaTime)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}