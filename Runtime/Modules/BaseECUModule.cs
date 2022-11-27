using UnityEngine;

namespace Drifter.Modules
{
    public abstract class BaseECUModule
    {
        [field: SerializeField, Range(1, 10)] public byte Strength { get; set; } = 1;
    }
}