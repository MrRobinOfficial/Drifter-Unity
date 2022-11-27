using Drifter.Components;
using UnityEngine;

namespace Drifter.Extensions
{
    public static partial class DrifterExtensions
    {
        public static float GetCurrentTorque(this EngineComponent engine) =>
            engine.GetMaxEffectiveTorque(engine.GetRPM());

        public static float GetCurrentPower(this EngineComponent engine) =>
            engine.GetPowerFromTorque(engine.GetCurrentTorque());

        public static string GetCurrentGearText(this GearboxComponent gearbox) => 
            gearbox.GetGearText(gearbox.GearIndex);

        public static string GetGearText(this GearboxComponent gearbox, int index) => index == 0
                ? "N"
                : (index % (gearbox.ForwardGears.Length + 1)) switch
                {
                    < -1 => $"R [{GetGearText(gearbox, Mathf.Abs(index))}]",
                    -1 => "R",
                    1 => $"{index}st",
                    2 => $"{index}nd",
                    3 => $"{index}rd",
                    _ => $"{index}th",
                };
    }
}