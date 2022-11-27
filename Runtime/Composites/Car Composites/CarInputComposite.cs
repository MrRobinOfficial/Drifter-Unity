using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

using static Drifter.Utility.MathUtility;

namespace Drifter.Composites.CarComposites
{
    [AddComponentMenu("Tools/Drifter/Composites/Car Input [Composite]"), DisallowMultipleComponent]
    public class CarInputComposite : BaseCarComposite
    {
        [Header("References")]
        [SerializeField] InputActionAsset m_InputActionAsset = default;

        [Header("Force Feedback Settings")]
        [SerializeField, Range(0f, 1f)] float m_FFBModifier = 1f;

        [Header("Keyboard & Gamepad Settings")]
        public AnimationCurve steerCurve = new(DefaultSteerCurve.keys);
        public float steerSpeed = 30f;

        [Header("Keyboard Input")]
        [SerializeField] Key m_ToggleTelemetry = Key.T;

        private static readonly AnimationCurve DefaultSteerCurve = AnimationCurve.EaseInOut(timeStart: 0f, valueStart: 0f, timeEnd: 1f, valueEnd: 1f);

        private InputActionMap vehicleInput = default;
        private InputActionMap carInput = default;

        /* VEHICLE INPUT */
        private InputAction refreshAction;
        private InputAction honkAction;
        private InputAction respawnAction;
        private InputAction setSpawnkAction;

        /* CAR INPUT */
        private InputAction steerAction;
        private InputAction throttleAction;
        private InputAction brakeAction;
        private InputAction clutchAction;
        private InputAction handbrakeAction;
        private InputAction shiftUpAction;
        private InputAction shiftDownAction;
        private InputAction starterAction;
        private InputAction toggleHandbrakeAction;

        protected override void Awake()
        {
            base.Awake();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            vehicleInput = m_InputActionAsset.FindActionMap("Vehicle");
            carInput = m_InputActionAsset.FindActionMap("Car");

            refreshAction = vehicleInput.FindAction("Refresh");
            honkAction = vehicleInput.FindAction("Honk");
            respawnAction = vehicleInput.FindAction("Respawn");
            setSpawnkAction = vehicleInput.FindAction("Set Spawn");

            steerAction = carInput.FindAction("Steering");
            throttleAction = carInput.FindAction("Throttle");
            brakeAction = carInput.FindAction("Brake");
            clutchAction = carInput.FindAction("Clutch");
            handbrakeAction = carInput.FindAction("Handbrake");
            shiftUpAction = carInput.FindAction("Shift Up");
            shiftDownAction = carInput.FindAction("Shift Down");
            starterAction = carInput.FindAction("Starter");
            toggleHandbrakeAction = carInput.FindAction("Toggle Handbrake");

            //respawnAction.performed += RespawnAction_performed;
            //setSpawnkAction.performed += SetSpawnkAction_performed;

            toggleHandbrakeAction.performed += ToggleHandbrake_performed;
            shiftUpAction.performed += ShiftUp_performed;
            shiftDownAction.performed += ShiftDown_performed;
        }

        private void OnDestroy()
        {
            //respawnAction.performed -= RespawnAction_performed;
            //setSpawnkAction.performed -= SetSpawnkAction_performed;

            toggleHandbrakeAction.performed -= ToggleHandbrake_performed;
            shiftUpAction.performed -= ShiftUp_performed;
            shiftDownAction.performed -= ShiftDown_performed;
        }

        private void OnEnable()
        {
            vehicleInput.Enable();
            carInput.Enable();
        }

        private void OnDisable()
        {
            vehicleInput.Disable();
            carInput.Disable();
        }

        private bool isStarting;
        private float starterTime;
        private bool isHandbrakeToggle;

        private float smoothSteerInput;
        private float steerInputVelocity;

        private void Update()
        {
            var rawSteerInput = steerAction.ReadValue<float>();
            //var steerActionDevice = steerAction.activeControl?.device;

            var steerInput = steerCurve.Evaluate(Mathf.Abs(rawSteerInput)) * Sign(rawSteerInput);
            smoothSteerInput = Mathf.Lerp(smoothSteerInput, steerInput, steerSpeed * Time.deltaTime);
            carVehicle.SetSteerInput(smoothSteerInput);

            carVehicle.SetThrottleInput(throttleAction.ReadValue<float>());
            carVehicle.SetBrakeInput(brakeAction.ReadValue<float>());
            carVehicle.SetClutchInput(clutchAction.ReadValue<float>());
            carVehicle.SetHandbrakeInput(
                Mathf.Max(handbrakeAction.ReadValue<float>(), isHandbrakeToggle ? 1f : 0f));

            if (Input.GetKeyDown(KeyCode.T))
            {
                if (carVehicle.Engine.IsRunning)
                    carVehicle.Engine.Shutoff();
                else
                    carVehicle.Engine.Startup();
            }
        }

        //private void RespawnAction_performed(InputAction.CallbackContext ctx) => carVehicle.Respawn();

        //private void SetSpawnkAction_performed(InputAction.CallbackContext ctx)
        //{
        //    if (!carVehicle.IsGrounded)
        //        return;

        //    carVehicle.SetSpawnLocation(transform.position, transform.rotation);
        //}

        //private void Update()
        //{
        //    if (Gamepad.current != null)
        //        HandleGamepad();

        //    if (Keyboard.current != null)
        //        HandleKeyboard();

        //    var rawSteerInput = steerAction.ReadValue<float>();
        //    var steerActionDevice = steerAction.activeControl?.device;

        //    var steerInput = steerCurve.Evaluate(Mathf.Abs(rawSteerInput)) * Sign(rawSteerInput);
        //    smoothSteerInput = Mathf.Lerp(smoothSteerInput, steerInput, steerSpeed * Time.deltaTime);
        //    carVehicle.RawSteerInput = smoothSteerInput;

        //    carVehicle.RawThrottleInput = throttleAction.ReadValue<float>();
        //    carVehicle.RawBrakeInput = brakeAction.ReadValue<float>();
        //    carVehicle.RawClutchInput = clutchAction.ReadValue<float>();
        //    carVehicle.RawHandbrakeInput = Mathf.Max(handbrakeAction.ReadValue<float>(), isHandbrakeToggle ? 1f : 0f);

        //    HandleEngineInput();

        //    void HandleKeyboard()
        //    {
        //        var keyboard = Keyboard.current;

        //        if (keyboard[m_ToggleTelemetry].wasPressedThisFrame)
        //        {
        //            if (TryGetComponent(out CarTelemetryGUIComposite telemetryGUI))
        //                telemetryGUI.enabled = !telemetryGUI.enabled;
        //        }
        //    }

        //    void HandleGamepad()
        //    {
        //        var pad = Gamepad.current;

        //        if (pad[GamepadButton.Start].wasPressedThisFrame)
        //            Debug.Break();
        //    }

        //    void HandleEngineInput()
        //    {
        //        var engine = carVehicle.Engine;

        //        if (!isStarting && engine.IsRunning && starterAction.WasPressedThisFrame())
        //        {
        //            engine.Stop();
        //            ResetRumble();
        //        }
        //        else if (starterAction.IsPressed() && !isStarting && !engine.IsRunning)
        //        {
        //            starterTime = Time.unscaledTime + engine.StarterDuration;
        //            ApplyRumble(lowFrequency: 0.5f, highFrequency: 0.5f);
        //            isStarting = true;
        //        }
        //        else if (isStarting && starterAction.WasReleasedThisFrame())
        //        {
        //            isStarting = false;
        //            ResetRumble();
        //        }
        //        else if (isStarting && starterAction.IsPressed())
        //        {
        //            if (Time.unscaledTime >= starterTime && !engine.IsRunning)
        //            {
        //                engine.Startup();
        //                isStarting = false;
        //                ResetRumble();
        //            }
        //        }
        //    }

        //}

        //private void HandleECU()
        //{
        //    if (!carReference.ECU.IsNotNull)
        //        return;

        //    var ecu = carReference.ECU.Value;

        //    if (ecu.CruiseControl.IsNotNull)
        //    {
        //        var cruiseControl = ecu.CruiseControl.Value;

        //        if (Keyboard.current[Key.Home].wasPressedThisFrame)
        //            cruiseControl.Toggle();
        //    }

        //    if (ecu.SpeedLimiter.IsNotNull)
        //    {
        //        var speedLimiter = ecu.SpeedLimiter.Value;

        //        if (Keyboard.current[Key.End].wasPressedThisFrame)
        //            speedLimiter.Toggle();
        //    }
        //}

        public void ApplyRumble(float lowFrequency, float highFrequency)
        {
            if (Gamepad.current == null)
                return;

            Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);
        }

        private void PauseRumble()
        {
            if (Gamepad.current == null)
                return;

            Gamepad.current.PauseHaptics();
        }

        private void ResumeRumble()
        {
            if (Gamepad.current == null)
                return;

            Gamepad.current.ResumeHaptics();
        }

        private void ResetRumble()
        {
            if (Gamepad.current == null)
                return;

            Gamepad.current.ResetHaptics();
        }

        #region Input Callback

        private void ToggleHandbrake_performed(InputAction.CallbackContext ctx) => 
            isHandbrakeToggle = !isHandbrakeToggle;

        private void ShiftUp_performed(InputAction.CallbackContext ctx)
        {
            var type = carVehicle.Gearbox.Type;

            if (type == TransmissionType.Manual ||
                type == TransmissionType.AutomaticSequential)
                carVehicle.Gearbox.ShiftUp();
        }

        private void ShiftDown_performed(InputAction.CallbackContext ctx)
        {
            var type = carVehicle.Gearbox.Type;

            if (type == TransmissionType.Manual ||
                type == TransmissionType.AutomaticSequential)
                carVehicle.Gearbox.ShiftDown();
        }
        #endregion
    }
}