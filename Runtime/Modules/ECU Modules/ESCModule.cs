using Drifter.Components;
using Drifter.Vehicles;
using UnityEngine;

namespace Drifter.Modules.ECUModules
{
    /// <summary>
    /// ESC - (Electronic Stability Control) helps prevent loss of control in curves and emergency steering maneuvers by stabilizing your car when it begins to veer off your intended path. <a href="https://mycardoeswhat.org/">Click here for more info</a>
    /// </summary>
    [System.Serializable]
    public sealed class ESCModule : BaseECUModule
    {
        //[SerializeField] Preset m_Preset = Preset.Custom;

        //public override void Simulate(CarVehicle car, ECUComponent ecu, float deltaTime, ref float steerInput, ref float throttleInput, ref float brakeInput, ref float clutchInput, ref float handbrakeInput)
        //{
        //    throw new System.NotImplementedException();
        //}

        // SteerAngle - Angle of the front wheels off center
        // DesiredYawRate - Ideal rotation speed of the Steering input
        // YawRate - Left >0 Right <0
        // Speed - Speed of vehicle
        // DesiredRawRateAccel - Ideal acceleration input for turning
        // DesiredRawRateSteering - Ideal rotation speed for turning
    }
}