using Drifter.Extensions;
using Drifter.Utility;
using TMPro;
using UnityEngine;
using Drifter.Vehicles;
using Drifter.Misc;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Presets;
#endif

using static Drifter.Extensions.DrifterExtensions;

namespace Drifter.UGUI
{
    [AddComponentMenu("Tools/Drifter/UGUI/Car Dashboard [UGUI]"), DisallowMultipleComponent]
    public class CarDashboardUGUI : MonoBehaviour
    {
        private const string EMPTY_TEXT = "-";

        [System.Serializable]
        public class Needle
        {
            public Transform needle;
            public float minValue = 0.0f;
            public float maxValue = 200.0f;
            public float angleAtMinValue = 135.0f;
            public float angleAtMaxValue = -135.0f;

            public Quaternion GetAngles(float value)
            {
                var normValue = (value - minValue) / (maxValue - minValue);
                var angle = MathUtility.UnclampedLerp(angleAtMinValue, angleAtMaxValue, normValue);

                return Quaternion.Euler(x: 0, y: 0, angle);
            }

            public void SetValue(float value)
            {
                if (needle == null) 
                    return;

                needle.localRotation = GetAngles(value);
            }
        }

        [Header("References")]
        [SerializeField] CarVehicle m_CarVehicle = default;
#if UNITY_EDITOR
        [SerializeField] Transform m_Container = default;
#endif

        [Header("Needles")]
        [SerializeField] Optional<Needle> m_Speedometer = default;
        [SerializeField] Optional<Needle> m_Tachometer = default;

        [Header("Gauges")]
        [SerializeField] Optional<UGUIImage_Filler> m_ThrottleGauge = default;
        [SerializeField] Optional<UGUIImage_Filler> m_BrakeGauge = default;
        [SerializeField] Optional<UGUIImage_Filler> m_ClutchGauge = default;
        [SerializeField] Optional<UGUIImage_Filler> m_FuelGauge = default;
        [SerializeField] Optional<UGUIImage_Filler> m_TemperatureGauge = default;

        [Header("Texts")]
#if UNITY_EDITOR
        [SerializeField] Preset m_TextPreset = default;
#endif
        [SerializeField] Optional<TextMeshProUGUI> m_SpeedText = default;
        [SerializeField] Optional<TextMeshProUGUI> m_GearText = default;

        [Header("Warnings")]
        [SerializeField] GameObject m_EngineSymbol = default;
        [SerializeField] GameObject m_EngineCoolantSymbol = default;
        [SerializeField] GameObject m_BrakeSymbol = default;
        [SerializeField] GameObject m_BatterySymbol = default;
        [SerializeField] GameObject m_OilPressureSymbol = default;
        [SerializeField] GameObject m_TirePressureSymbol = default;
        [SerializeField] GameObject m_LowFuelSymbol = default;
        [SerializeField] GameObject m_ABSSymbol = default;
        [SerializeField] GameObject m_TCSSymbol = default;
        [SerializeField] GameObject m_ESPSymbol = default;
        [SerializeField] GameObject m_ShiftUpSymbol = default;
        [SerializeField] GameObject m_ShiftDownSymbol = default;

        [Header("Indications")]
        [SerializeField] Image m_IgnitionKey = default;
        [SerializeField] Image m_Horn = default;
        [SerializeField] Image m_SignalLeft = default;
        [SerializeField] Image m_SignalRight = default;

#if UNITY_EDITOR
        [ContextMenu("Generate Speedometer", isValidateFunction: false, priority: 1000150)]
        private void GenerateSpeedometer() { }

        [ContextMenu("Generate Tachometer", isValidateFunction: false, priority: 1000150)]
        private void GenerateTachometer()
        {
            //if (!m_Tachometer.IsNotNull)
            //    return;

            var tachoObj = new GameObject(name: "Tachometer", typeof(RectTransform))
                .GetComponent<RectTransform>();

            tachoObj.SetAsLastSibling();
            tachoObj.SetParent(m_Container);
            tachoObj.anchoredPosition = Vector2.zero;

            var prefab = GenerateTextTemplate(tachoObj);
            var tacho = m_Tachometer.Value;

            const int LABEL_COUNT = 10;

            var totalAngle = tacho.angleAtMinValue - tacho.angleAtMaxValue;

            for (int i = 0; i <= LABEL_COUNT; i++)
            {
                var obj = Instantiate(prefab, tachoObj.transform);
                var textComponent = obj.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);

                obj.transform.localPosition = Vector3.zero;
                textComponent.transform.localPosition = Vector3.zero;

                var angle = (float)i / LABEL_COUNT;
                var newAngle = tacho.angleAtMinValue - angle * totalAngle;

                obj.transform.eulerAngles = new Vector3(0f, 0f, newAngle);

                textComponent.transform.localEulerAngles = Vector3.zero;
                textComponent.SetText($"{i}");

                obj.gameObject.SetActive(true);
            }

            DestroyImmediate(prefab.gameObject);
        }

        [ContextMenu("Generate Odometer", isValidateFunction: false, priority: 1000150)]
        private void GenerateOdometer() { }

        private GameObject GenerateTextTemplate(Transform parent)
        {
            var rootObj = new GameObject(name: "Text-Prefab", typeof(RectTransform))
                .GetComponent<RectTransform>();

            rootObj.SetParent(parent);
            rootObj.anchoredPosition = Vector2.zero;

            rootObj.gameObject.SetActive(false);

            var textObj = new GameObject("Text-Component", 
                typeof(RectTransform), typeof(TextMeshProUGUI))
                .GetComponent<RectTransform>();

            const float X_COORD = 96f;

            textObj.SetParent(rootObj.transform);
            textObj.anchoredPosition = new Vector2(X_COORD, 0f);

            var textComponent = textObj.GetComponent<TextMeshProUGUI>();

            if (m_TextPreset != null)
                m_TextPreset.ApplyTo(textComponent);

            return rootObj.gameObject;
        }
#endif

        private const float INPUT_THRESHOLD = 0.925f;

        //speedometer.Set(m_Vehicle.GetSpeedInKph());
        //speedometer.Set(m_Vehicle.DataBus[DataBus.SPEED_KPH].Get<float>());
        //tachometer.Set(m_Vehicle[SPEED_KPH])

        private void SetAllSymbols(bool value)
        {
            m_EngineSymbol.SetActive(value);
            m_EngineCoolantSymbol.SetActive(value);
            m_BrakeSymbol.SetActive(value);
            m_BatterySymbol.SetActive(value);
            m_OilPressureSymbol.SetActive(value);
            m_TirePressureSymbol.SetActive(value);
            m_LowFuelSymbol.SetActive(value);
            m_ABSSymbol.SetActive(value);
            m_TCSSymbol.SetActive(value);
            m_ESPSymbol.SetActive(value);
            m_ShiftUpSymbol.SetActive(value);
            m_ShiftDownSymbol.SetActive(value);
        }

        private void SetAllGauges(float value)
        {
            if (m_FuelGauge.Enabled && m_FuelGauge.Value != null)
                m_FuelGauge.Value.FillAmount = value;

            if (m_TemperatureGauge.Enabled && m_TemperatureGauge.Value != null)
                m_TemperatureGauge.Value.FillAmount = value;

            if (m_ThrottleGauge.Enabled && m_ThrottleGauge.Value != null)
                m_ThrottleGauge.Value.FillAmount = value;

            if (m_BrakeGauge.Enabled && m_BrakeGauge.Value != null)
                m_BrakeGauge.Value.FillAmount = value;

            if (m_ClutchGauge.Enabled && m_ClutchGauge.Value != null)
                m_ClutchGauge.Value.FillAmount = value;
        }

        private void SetAllNeedles(float value)
        {
            if (m_Speedometer.Enabled)
                m_Speedometer.Value.SetValue(value);

            if (m_Tachometer.Enabled)
                m_Tachometer.Value.SetValue(value);
        }

        private void Awake()
        {
            m_GearText.TryGetValue()?.SetText(EMPTY_TEXT);
            m_SpeedText.TryGetValue()?.SetText(EMPTY_TEXT);

            m_Speedometer.TryGetValue()?.SetValue(0f);
            m_Tachometer.TryGetValue()?.SetValue(0f);

            SetAllSymbols(false);
            SetAllGauges(0f);
            SetAllNeedles(0f);
        }

        private void Update()
        {
            if (m_CarVehicle == null || !m_CarVehicle.isActiveAndEnabled)
            {
                m_Speedometer.TryGetValue()?.SetValue(value: 0f);
                m_Tachometer.TryGetValue()?.SetValue(value: 0f);

                m_GearText.TryGetValue()?.SetText(EMPTY_TEXT);
                m_SpeedText.TryGetValue()?.SetText(EMPTY_TEXT);

                return;
            }

            if (m_ThrottleGauge.Enabled && m_ThrottleGauge.Value != null)
                m_ThrottleGauge.Value.FillAmount = m_CarVehicle.GetThrottleInput();

            if (m_BrakeGauge.Enabled && m_BrakeGauge.Value != null)
                m_BrakeGauge.Value.FillAmount = m_CarVehicle.GetBrakeInput();

            if (m_ClutchGauge.Enabled && m_ClutchGauge.Value != null)
                m_ClutchGauge.Value.FillAmount = m_CarVehicle.GetClutchInput();

            var frameRatio = MathUtility.GetFrameRatio();
            var speed = Mathf.Max(m_CarVehicle.Speedometer.GetInterpolated(frameRatio), b: 0f);
            var tacho = Mathf.Max(m_CarVehicle.Tachometer.GetInterpolated(frameRatio), b: 0f);

            m_GearText.TryGetValue()?.SetText(m_CarVehicle.Gearbox.GetCurrentGearText());
            m_SpeedText.TryGetValue()?.SetText($"{speed:N0}");

            m_Speedometer.TryGetValue()?.SetValue(speed);
            m_Tachometer.TryGetValue()?.SetValue(tacho);

            var isBraking = m_CarVehicle.GetHandbrakeInput() > INPUT_THRESHOLD;
            m_BrakeSymbol.SetActive(isBraking);
        }
    }
}