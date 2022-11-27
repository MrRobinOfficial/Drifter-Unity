using Drifter.Components;
using Drifter.Modules;
using UnityEngine;

namespace Drifter.Vehicles
{
    [AddComponentMenu("Tools/Drifter/Vehicles/Motorbike [Vehicle]"), DisallowMultipleComponent]
    public class MotorbikeVehicle : BaseVehicle
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            DisplayName = "Buell XB12S";
        }
    }
}