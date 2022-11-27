using UnityEngine;

namespace Drifter.Modules
{
    [System.Serializable]
    public sealed class SuperchargerModule : BaseModule
    {
        [field: Header("Settings")]
        [field: SerializeField] public float Inertia { get; set; } = 1f;

        public float RPM { get; private set; } = 0f;
    } 
}