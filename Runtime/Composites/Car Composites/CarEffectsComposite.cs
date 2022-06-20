using Drifter.Extensions;
using Drifter.Handlers;
using Drifter.Vehicles;
using UnityEngine;

namespace Drifter.Composites.CarComposites
{
    [AddComponentMenu("Tools/Drifter/Composites/Car Effects [Composite]"), DisallowMultipleComponent]
    [RequireComponent(typeof(CarVehicle))]
    public class CarEffectsComposite : MonoBehaviour
    {
        [System.Serializable]
        public class SteeringWheel
        {
            public Transform steeringWheel;
            public AxisDirection direction;
        }

        [SerializeField] Optional<SteeringWheel> m_SteeringWheel = default;

        [Header("VFX")]
        [SerializeField] TrailRenderer m_TireSkidVFX = default;
        [SerializeField] ParticleSystem m_TireSmokeVFX = default;

        private CarVehicle carVehicle = default;

        private void Awake()
        {
            carVehicle = GetComponent<CarVehicle>();

            //foreach (var wheel in carVehicle.WheelArray)
            //{
            //    var handler = wheel.gameObject.AddComponent<WheelVFXHandler>();
            //    handler.Init(m_TireSkidVFX, m_TireSmokeVFX);
            //}
        }

        private void OnValidate() => RefreshVisual();

        private void RefreshVisual()
        {
            if (!m_SteeringWheel.Enabled)
                return;

            var steeringWheel = m_SteeringWheel.Value.steeringWheel;

            if (steeringWheel != null)
                steeringWheel.gameObject.SetActive(m_SteeringWheel.Enabled);
        }

        private float prevAngle;

        private void Update()
        {
            if (m_SteeringWheel.Enabled && m_SteeringWheel.Value.steeringWheel != null)
            {
                var steeringWheel = m_SteeringWheel.Value;
                var axis = steeringWheel.steeringWheel.GetAxisDirection(steeringWheel.direction);

                var angle = carVehicle.GetSteerAngle();
                var angleDelta = angle - prevAngle;

                steeringWheel.steeringWheel.Rotate(axis, angleDelta);

                prevAngle = angle;
            }
        }
    }
}