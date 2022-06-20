#if ENABLE_CINEMACHINE
using UnityEngine;
using Cinemachine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Drifter.Samples.Cinemachine
{
    [AddComponentMenu(""), DisallowMultipleComponent]
    public class CinemachineCameraController : CinemachineExtension
    {
#if ENABLE_INPUT_SYSTEM
        [Header("References")]
        [SerializeField] InputActionReference m_ActionReference = default;

        private InputAction switchAction = default;
#endif

        protected override void Awake()
        {
            base.Awake();

#if ENABLE_INPUT_SYSTEM
            switchAction = m_ActionReference.action;
            switchAction.performed += OnSwitchAction;
#endif
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

#if ENABLE_INPUT_SYSTEM
            switchAction.performed -= OnSwitchAction;
#endif
        }

        protected override void OnEnable()
        {
            base.OnEnable();

#if ENABLE_INPUT_SYSTEM
            switchAction.Enable();
#endif
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            switchAction.Disable();
#endif
        }

        public enum CameraState : byte
        {
            Orbit,
            POV,
            Hood,
        }

        public CameraState CurrentState { get; private set; } = CameraState.Orbit;

        private const int STATE_LENGTH = 3;

        private static readonly int ORBIT_STATE = Animator.StringToHash("Orbit State");
        private static readonly int POV_STATE = Animator.StringToHash("POV State");
        private static readonly int HOOD_STATE = Animator.StringToHash("Hood State");

#if ENABLE_INPUT_SYSTEM
        private void OnSwitchAction(InputAction.CallbackContext ctx) => CurrentState = (CameraState)(((int)CurrentState + 1) % STATE_LENGTH);

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref global::Cinemachine.CameraState state, float deltaTime)
        {
            if (vcam is not CinemachineStateDrivenCamera)
                return;

            var cam = (CinemachineStateDrivenCamera)vcam;

            if (cam.IsBlending)
                return;

            switch (CurrentState)
            {
                case CameraState.Orbit:
                    cam.m_AnimatedTarget.Play(ORBIT_STATE);
                    break;

                case CameraState.POV:
                    cam.m_AnimatedTarget.Play(POV_STATE);
                    break;

                case CameraState.Hood:
                    cam.m_AnimatedTarget.Play(HOOD_STATE);
                    break;
            }
        }
#endif
    }
}

#endif