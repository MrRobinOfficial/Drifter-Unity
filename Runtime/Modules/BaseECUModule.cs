using Drifter.Components;
using Drifter.Vehicles;
using NaughtyAttributes;
using UnityEngine;

namespace Drifter.Modules
{
    public enum Preset : byte
    {
        Custom,
        Weak,
        Medium,
        Strong,
    }

    public abstract class BaseECUModule
    {
        protected const float INPUT_THRESHOLD = 0.125f;

        protected const float BRAKE_THRESHOLD = 0.125f;
        protected const float THROTTLE_THRESHOLD = 0.125f;

        [field: SerializeField, Range(1, 10)] public byte Strength { get; set; } = 1;

        [ShowNativeProperty] public bool IsEnabled { get; protected set; }

        public abstract void Simulate(CarVehicle car, ECUComponent ecu, float deltaTime, ref float steerInput, ref float throttleInput, ref float brakeInput, ref float clutchInput, ref float handbrakeInput);
    }
}