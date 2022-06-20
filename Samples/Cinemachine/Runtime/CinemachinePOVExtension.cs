using UnityEngine;
using NaughtyAttributes;
using System.Collections;
using Cinemachine;
using UnityEngine.InputSystem;
using Drifter.Utility;

namespace Drifter.Samples.Cinemachine
{
    [AddComponentMenu(""), DisallowMultipleComponent]
    public class CinemachinePOVExtension : CinemachineExtension
    {
        [Header("References")]
        [SerializeField] InputActionReference m_ActionReference = default;

        [field: Header("Sensitivity Settings")]
        [field: SerializeField] public float HorizontalSensitivity { get; set; } = 300f;
        [field: SerializeField] public float VerticalSensitivity { get; set; } = 300f;

        [field: Header(header: "Clamps Settings")]
        [field: SerializeField, MinMaxSlider(-180f, 180f)] public Vector2 HorizontalClamp { get; set; } = new(-110f, 110f);
        [field: SerializeField, MinMaxSlider(-90f, 90f)] public Vector2 VerticalClamp { get; set; } = new(-70f, 70f);

        [field: Header("Re-center Settings")]
        [field: SerializeField] public float RecenterWaitTime { get; set; } = 1.5f;
        [field: SerializeField] public float RecenterTime { get; set; } = 0.5f;

        private InputAction lookAction;

        protected override void Awake()
        {
            base.Awake();

            lookAction = m_ActionReference.action.Clone();

            if (Mouse.current != null)
                lookAction.AddBinding(Mouse.current.delta);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (lookAction != null)
                lookAction.Enable();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            if (lookAction != null)
                lookAction.Disable();
        }

        private float yawAngle;
        private float pitchAngle;

        private float inputXPrev;
        private float inputYPrev;

        private float waitTimer;
        private Coroutine lookCoroutine = null;

        private float inputX;
        private float inputY;
        private float inputXVelocity;
        private float inputYVelocity;

        private IEnumerator ResetLook(Transform target, float duration = 0.5f)
        {
            var time = 0f;

            var startRot = transform.rotation;
            var targetRot = Quaternion.LookRotation(target.forward);

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;
                transform.rotation = Quaternion.Slerp(startRot, targetRot, time);

                var rot = transform.localRotation;
                pitchAngle = rot.x;
                yawAngle = rot.y;

                yield return null;
            }

            transform.rotation = targetRot;
            lookCoroutine = null;
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (!Application.isPlaying)
                return;

            if (!vcam.Follow ||
                stage != CinemachineCore.Stage.Aim)
                return;

            var lookInput = lookAction.ReadValue<Vector2>();

            var targetX = lookInput.x * HorizontalSensitivity * Time.deltaTime;
            var targetY = lookInput.y * VerticalSensitivity * Time.deltaTime;

            const float SMOOTH_TIME = 0.1f;
            const float MAX_SPEED = 90f;

            inputX = Mathf.SmoothDamp(inputX, targetX, ref inputXVelocity, SMOOTH_TIME, MAX_SPEED, deltaTime);

            inputY = Mathf.SmoothDamp(inputY, targetY, ref inputYVelocity, SMOOTH_TIME, MAX_SPEED, deltaTime);

            //var inputXDelta = inputX - inputXPrev;
            //var inputYDelta = inputY - inputYPrev;

            var rot = state.RawOrientation.eulerAngles;
            var invertedModifier = -Vector3.Dot(vcam.Follow.up, Vector3.down);

            yawAngle += inputX * invertedModifier;
            yawAngle = DrifterMathUtility.ClampAngle(yawAngle, HorizontalClamp.x, HorizontalClamp.y);

            pitchAngle -= inputY * invertedModifier;
            pitchAngle = DrifterMathUtility.ClampAngle(pitchAngle, VerticalClamp.x, VerticalClamp.y);

            state.RawOrientation = Quaternion.Euler(rot.x + pitchAngle, rot.y + yawAngle, rot.z);

            //inputXPrev = inputX;
            //inputYPrev = inputY;

            //if (IsLooking())
            //{
            //if (lookCoroutine != null)
            //{
            //    StopCoroutine(lookCoroutine);
            //    lookCoroutine = null;
            //}

            //waitTimer = 0f;

            //transform.localRotation = Quaternion.Euler(xRot, yRot, z: 0f);
            //    state.RawOrientation = Quaternion.Euler(xRot, yRot, z: 0f);
            //}
            //else
            //{
            //    if (waitTimer < RecenterWaitTime)
            //    {
            //        waitTimer += Time.deltaTime;

            //        if (waitTimer > RecenterWaitTime)
            //        {
            //            waitTimer = RecenterWaitTime;

            //            //if (lookCoroutine == null)
            //            //    lookCoroutine = StartCoroutine(ResetLook(transform.root, RecenterTime));
            //        }
            //    }
            //}

            //inputXPrev = inputX;
            //inputYPrev = inputY;

            //bool IsLooking() => inputXDelta != 0f ||
            //    inputYDelta != 0f;
        }
    }
}