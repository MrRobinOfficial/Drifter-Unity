using Drifter.Components;
using UnityEngine;

namespace Drifter.Modules
{
    [System.Serializable]
    public sealed class RevlimiterModule : BaseModule
    {
        public enum RevType : byte
        {
            SoftCut,
            HardCut,
            TimeBased,
        }

        [field: SerializeField] public RevType Type { get; set; } = RevType.HardCut;

        public bool IsLimiting { get; private set; } = false;

        public void Simulate(float deltaTime, float engineRPM, ref float throttleInput) { }
    }
}