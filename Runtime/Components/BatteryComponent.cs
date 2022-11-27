using UnityEngine;

namespace Drifter.Components
{
    [System.Serializable]
    public class BatteryComponent : BaseComponent
    {
        [field: SerializeField] public uint VoltCapacity { get; set; } = 12;

        public float GetPowerNormalized() => 1f;

        public override void OnEnable(BaseVehicle vehicle) { }
        public override void OnDisable(BaseVehicle vehicle) { }

        #region Data Saving
        public override void Load(FileData data) => throw new System.NotImplementedException();
        public override FileData Save() => throw new System.NotImplementedException();

        #endregion
    }
}