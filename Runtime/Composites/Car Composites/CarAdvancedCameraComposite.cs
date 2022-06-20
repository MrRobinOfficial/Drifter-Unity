using Cinemachine;
using Drifter.Attributes;
using Drifter.Data;
using Drifter.Handlers;
using Drifter.Vehicles;
using NaughtyAttributes;
using UnityEngine;

namespace Drifter.Composites.CarComposites
{
    [AddComponentMenu("Tools/Drifter/Composites/Car Advanced Camera [Composite]"), DisallowMultipleComponent]
    [RequireComponent(typeof(CarVehicle))]
    public sealed class CarAdvancedCameraComposite : BaseCameraComposite
    {
        [Header("References")]
        [SerializeField] CinemachineStateDrivenCamera m_StateDrivenCam = default;
        [SerializeField, Required] CinemachineFreeLook m_OrbitCam = default;
        [SerializeField, Required] CinemachineFreeLook m_POVCam = default;
        [SerializeField, Required] CinemachineFreeLook m_HoodCam = default;

        [Header("General Settings")]
        [SerializeField, ExposedData] CameraPreset m_Preset = default;

        protected override void Awake()
        {
            base.Awake();

            var handler = gameObject.AddComponent<CinemachineCameraHandler>();
            handler.Init(carVehicle, m_Preset, m_StateDrivenCam);
        }

        private const int LOW_PRIORITY = 7;
        private const int HIGH_PRIORITY = 10;

        protected override void OnViewSwitched(CameraType type)
        {
            m_OrbitCam.m_Priority = type != CameraType.Orbit ? LOW_PRIORITY : HIGH_PRIORITY;
            m_POVCam.m_Priority = type != CameraType.POV ? LOW_PRIORITY : HIGH_PRIORITY;
            m_HoodCam.m_Priority = type != CameraType.Hood ? LOW_PRIORITY : HIGH_PRIORITY;

            print(type);
        }
    }
}