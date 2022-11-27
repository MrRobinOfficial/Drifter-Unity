using Drifter.Extensions;
using Drifter.Handlers;
using Drifter.Utility;
using Drifter.Vehicles;
using NaughtyAttributes;
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

        [System.Serializable]
        public class Driveshaft
        {
            public Transform driveshaft;
            public AxisDirection direction;
        }

        [SerializeField] Optional<SteeringWheel> m_SteeringWheel = default;
        [SerializeField, Tooltip("Also known as 'propeller-shaft', is a component for transmitting mechanical power and torque and rotation, usually used to connect other components of a drivetrain that cannot be connected directly because of distance or the need to allow for relative movement between them.\n\n[NOTE: Only for visual purposes]")] Optional<Driveshaft> m_Driveshaft = default;

        [Header("Exhaust Settings")]
        [SerializeField] float m_ExhaustRate = 5f;
        [SerializeField, Label("Exhaust VFXs")] ParticleSystem[] m_ExhaustVFXs = default;
        [SerializeField] ParticleSystem m_BackfireVFX = default;

        [Header("Tire VFX")]
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
            var steeringWheel = (!m_SteeringWheel)?.steeringWheel;

            if (steeringWheel != null)
                steeringWheel.gameObject.SetActive(m_SteeringWheel.Enabled);
        }

        private float prevSteerAngle;
        private float prevDriveAngle;

        private void Update()
        {
            if (m_Driveshaft.Enabled && m_Driveshaft.Value.driveshaft != null)
            {
                var data = m_Driveshaft.Value;
                var axis = data.driveshaft.GetAxisDirection(data.direction);

                var angle = carVehicle.DriveShaftVelocity * Mathf.Rad2Deg;
                var angleDelta = angle - prevDriveAngle;

                data.driveshaft.Rotate(axis, angleDelta);

                prevDriveAngle = angle;
            }

            if (m_SteeringWheel.Enabled && m_SteeringWheel.Value.steeringWheel != null)
            {
                var steeringWheel = m_SteeringWheel.Value;
                var axis = steeringWheel.steeringWheel.GetAxisDirection(steeringWheel.direction);

                var angle = carVehicle.GetSteerAngle();
                var angleDelta = angle - prevSteerAngle;

                steeringWheel.steeringWheel.Rotate(axis, angleDelta);

                prevSteerAngle = angle;
            }

            var engineRPM = carVehicle.Engine.GetRPM();
            var exhaustModifier = Mathf.Max(MathUtility.Sign(engineRPM), 
                engineRPM / carVehicle.Engine.MaxRPM);

            for (int i = 0; i < m_ExhaustVFXs.Length; i++)
            {
                var emission = m_ExhaustVFXs[i].emission;
                emission.rateOverTime = m_ExhaustRate * exhaustModifier;
            }
        }
    }
}