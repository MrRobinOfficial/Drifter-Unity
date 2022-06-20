using Drifter.Components;
using Drifter.Vehicles;
using UnityEngine;

namespace Drifter.Modules
{
    [System.Serializable]
    public sealed class HSAModule : BaseECUModule
    {
        [SerializeField] float m_InclineAngle = 3f;

        public override void Simulate(CarVehicle car, ECUComponent ecu, float deltaTime, ref float steerInput, ref float throttleInput, ref float brakeInput, ref float clutchInput, ref float handbrakeInput) => throw new System.NotImplementedException();

        //public override void Init(BaseVehicle vehicle) { }

        //public override void Simulate(float deltaTime) { }
    }
}