using Drifter.Components;
using Drifter.Vehicles;
using UnityEngine;

namespace Drifter.Modules
{
    [System.Serializable]
    public sealed class ESCModule : BaseECUModule
    {
        [SerializeField] Preset m_Preset = Preset.Custom;

        public override void Simulate(CarVehicle car, ECUComponent ecu, float deltaTime, ref float steerInput, ref float throttleInput, ref float brakeInput, ref float clutchInput, ref float handbrakeInput)
        {
            throw new System.NotImplementedException();
        }
    }
}