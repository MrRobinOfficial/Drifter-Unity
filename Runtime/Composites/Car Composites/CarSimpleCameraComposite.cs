using Drifter.Data;
using Drifter.Handlers;
using Drifter.Vehicles;
using NaughtyAttributes;
using UnityEngine;

namespace Drifter.Composites.CarComposites
{
    [AddComponentMenu("Tools/Drifter/Composites/Car Simple Camera [Composite]"), DisallowMultipleComponent]
    [RequireComponent(typeof(CarVehicle))]
    public sealed class CarSimpleCameraComposite : BaseCameraComposite
    {
        [Header("References")]
        [SerializeField, Required] Transform m_OrbitPivot = default;
        [SerializeField, Required] Transform m_POVPivot = default;
        [SerializeField, Required] Transform m_HoodPivot = default;

        [Header("General Settings")]
        [SerializeField, Expandable] CameraPreset m_Preset = default;

        public Transform CamTarget { get; private set; } = null;

        protected override void Awake()
        {
            base.Awake();

            var handler = gameObject.AddComponent<CameraHandler>();
            handler.Init(carVehicle, m_Preset, Camera.main);
        }

        protected override void OnViewSwitched(CameraType type) => CamTarget = type switch
        {
            CameraType.Orbit => m_OrbitPivot,
            CameraType.POV => m_POVPivot,
            CameraType.Hood => m_HoodPivot,
            _ => m_OrbitPivot,
        };
    }
}