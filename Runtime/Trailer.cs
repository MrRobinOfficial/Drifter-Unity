using UnityEngine;

namespace Drifter
{
    public class Trailer : MonoBehaviour, IHookable
    {
        [field: SerializeField] public uint Weight { get; set; } = 500;
    }
}