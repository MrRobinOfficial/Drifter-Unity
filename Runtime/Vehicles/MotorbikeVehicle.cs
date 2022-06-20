using Drifter.Components;
using Drifter.Modules;
using UnityEngine;

namespace Drifter.Vehicles
{
    [AddComponentMenu("Tools/Drifter/Vehicles/Motorbike [Vehicle]"), DisallowMultipleComponent]
    public class MotorbikeVehicle : BaseVehicle
    {
        [field: Header("References")]
        [field: SerializeField] public WheelBehaviour FrontWheel { get; private set; }
        [field: SerializeField] public WheelBehaviour RearWheel { get; private set; }

        [field: Header("Components")]
        [field: SerializeField] public ECUComponent ECU { get; private set; }
        [field: SerializeField] public EngineComponent Engine { get; private set; }
        [field: SerializeField] public ClutchComponent Clutch { get; private set; }
        [field: SerializeField] public GearboxComponent Gearbox { get; private set; }

        [field: Header("Modules")]
        [field: SerializeField] public FuelModule Fuel { get; private set; }

        protected override void OnInit()
        {

        }

        protected override void OnSimulate(float deltaTime) { }

        protected override void OnFixedSimulate(float deltaTime)
        {

        }

        protected override void OnShutdown() { }

        #region Data System

        public override void LoadData(FileData data)
        {

        }

        public override FileData SaveData() => FileData.Empty;

        #endregion
    }
}