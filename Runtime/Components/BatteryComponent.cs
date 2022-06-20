using UnityEngine;

namespace Drifter.Components
{
    [System.Serializable]
    public class BatteryComponent : BaseComponent
    {
        [field: SerializeField] public uint VoltCapacity { get; set; } = 12;

        public float GetPowerNormalized() => 1f;

        public override void Init(BaseVehicle vehicle) { }

        public override void Simulate(float deltaTime, IVehicleData data = null) { }

        public override void Shutdown() { }

        #region Data Saving
        public override void LoadData(FileData data) => throw new System.NotImplementedException();
        public override FileData SaveData() => throw new System.NotImplementedException();

        #endregion
    }
}