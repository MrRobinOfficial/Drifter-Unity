using Drifter.Extensions;
using Drifter.Utility;
using TMPro;
using UnityEngine;

namespace Drifter.UGUI
{
    [AddComponentMenu("Tools/Drifter/UGUI/Dashboard [UGUI]"), DisallowMultipleComponent]
    public class DashboardUGUI : MonoBehaviour
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

            public void SetValue(float value)
            {
                if (needle == null) 
                    return;

                var normValue = (value - minValue) / (maxValue - minValue);
                var angle = DrifterMathUtility.UnclampedLerp(angleAtMinValue, angleAtMaxValue, normValue);

                needle.localRotation = Quaternion.Euler(x: 0, y: 0, angle);
            }
        }

        [Header("References")]
        [SerializeField] BaseVehicle m_Vehicle = default;

        [Header("Gauges")]
        [SerializeField] Optional<Needle> m_Speedometer = default;
        [SerializeField] Optional<Needle> m_Tachometer = default;

        [Header("Texts")]
        [SerializeField] Optional<TextMeshProUGUI> m_SpeedText = default;
        [SerializeField] Optional<TextMeshProUGUI> m_GearText = default;

        [Header("Symbols")]
        [SerializeField] GameObject m_StalledSymbol = default;
        [SerializeField] GameObject m_WarningSymbol = default;
        [SerializeField] GameObject m_HandbrakeSymbol = default;

        private InterpolatedFloat speedometer = new();
        private InterpolatedFloat tachometer = new();

#if UNITY_EDITOR
        [ContextMenu("Gauges/Generate Speedometer")]
        private void GenerateSpeedometer() { }

        [ContextMenu("Gauges/Generate Tachometer")]
        private void GenerateTachometer() { }

        [ContextMenu("Gauges/Generate Odometer")]
        private void GenerateOdometer() { }
#endif

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            
        }

        private void LateUpdate()
        {
            if (m_Vehicle == null || !m_Vehicle.isActiveAndEnabled)
            {
                m_Speedometer.TryGetValue()?.SetValue(value: 0f);
                m_Tachometer.TryGetValue()?.SetValue(value: 0f);

                speedometer.Reset(value: 0f);
                tachometer.Reset(value: 0f);

                m_GearText.TryGetValue()?.SetText(EMPTY_TEXT);
                m_SpeedText.TryGetValue()?.SetText(EMPTY_TEXT);

                return;
            }

            var speed = Mathf.Max(speedometer.GetInterpolated(), b: 0f);
            var tacho = Mathf.Max(tachometer.GetInterpolated(), b: 0f);

            m_Speedometer.TryGetValue()?.SetValue(speed);
            m_Tachometer.TryGetValue()?.SetValue(tacho);
        }
    }
}