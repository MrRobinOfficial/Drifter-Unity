using Drifter.Components;
using Drifter.Vehicles;
using UnityEngine;

namespace Drifter.Modules.ECUModules
{
    /// <summary>
    /// <para>ABS - (Anti-Lock Braking System) helps you steer in emergencies by restoring traction to your tires.</para>
    /// <para>NOTE: It may not shorten the stopping distance</para>
    /// </summary>
    [System.Serializable]
    public sealed class ABSModule : BaseECUModule
    {
        [field: SerializeField, Range(0f, 1f)] public float SlipThreshold { get; set; } = 0.1f;
        [field: SerializeField] public RangeFloat SpeedRange { get; set; } = 
            new(minLimit: 0f, maxLimit: 1f);

        //[SerializeField] Preset m_Preset = Preset.Custom;
        //[SerializeField] float m_MinAngularVelocity = 6f;
        //[SerializeField] float m_MaxAngularVelocity = float.PositiveInfinity;
        //[SerializeField, Range(0f, 1f)] float m_SlipThreshold = 0.1f;

        //public override void Simulate(CarVehicle car, ECUComponent ecu, float deltaTime, ref float steerInput, ref float throttleInput, ref float brakeInput, ref float clutchInput, ref float handbrakeInput)
        //{
        //    if (brakeInput <= BRAKE_THRESHOLD)
        //    {
        //        IsEnabled = false;
        //        return;
        //    }

        //    for (int i = 0; i < car.WheelArray.Length; i++)
        //    {
        //        var wheel = car.WheelArray[i];
        //        var slipRatio = Mathf.Abs(wheel.SlipRatio);
        //        var angularVelocity = Mathf.Abs(wheel.AngularVelocity);

        //        //if (!wheel.IsLocked ||
        //        //    angularVelocity <= m_MinAngularVelocity ||
        //        //    angularVelocity >= m_MaxAngularVelocity)
        //            continue;

        //        var slip = slipRatio;
        //        Debug.Log(slip);

        //        if (slip > 0f && !IsEnabled)
        //            IsEnabled = true;

        //        if (slip < m_SlipThreshold && IsEnabled)
        //            IsEnabled = false;
        //    }

        //    if (IsEnabled)
        //        brakeInput = 0f;
        //}
    }
}