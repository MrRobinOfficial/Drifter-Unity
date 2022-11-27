#define DRAW_RAYCAST
#undef DRAW_RAYCAST

using UnityEngine;
using Drifter.Data;
using Drifter.Misc;

using static Drifter.Extensions.DrifterExtensions;
using static Drifter.Extensions.WheelExtensions;
using static Drifter.Utility.MathUtility;
using NaughtyAttributes;

namespace Drifter.Components
{
    [AddComponentMenu("Tools/Drifter/Behaviours/Wheel [Behaviour]"), DisallowMultipleComponent]
    public sealed class WheelBehaviour : BaseVehicleBehaviour
    {
        private const float SLOPE_THRESHOLD = 50f;

        [field: SerializeField, Expandable] public SuspensionPreset Suspension { get; set; } = default;
        [field: SerializeField, Expandable] public BaseTireFrictionPreset TireFriction { get; set; } = default;
        [field: SerializeField] public Transform Visual { get; set; } = default;

        //[field: SerializeField] public RaycastCastType RaycastCast { get; set; } = RaycastCastType.Single;
        [field: SerializeField] public Vector3 Center { get; set; } = Vector3.zero;
        [field: SerializeField] public LayerMask CollideWith { get; set; } = 1;
        [field: SerializeField] public float Mass { get; set; } = 30f;

        [SerializeField] float m_Width = 0.15f;
        [SerializeField] float m_Radius = 0.35f;

        public float Width
        {
            get => m_Width * base.transform.GetScale();
            set => m_Width = value;
        }

        public float Radius
        {
            get => m_Radius * base.transform.GetScale();
            set => m_Radius = value;
        }

        [field: SerializeField] public float RollingResistance { get; set; } = 0.05f;
        [field: SerializeField] public float MaxBrakeTorque { get; set; } = 3500f;

        [HideInInspector] public RaycastHit Hit => _hit;
        public Vector3 VelocityAtWheel { get; private set; }
        public float SteerAngle { get; set; }
        public float ToeAngle { get; set; }
        public float CamberAngle { get; set; }
        public float CasterAngle { get; set; }
        public float MotorTorque { get; private set; }
        public float BrakeTorque { get; private set; }
        public float TractionTorque { get; private set; }
        public float SlipRatio { get; private set; }
        public float SlipAngle { get; private set; }
        public float NormalForce { get; private set; }
        public float LongitudinalForce { get; private set; }
        public float LateralForce { get; private set; }
        public float AligningTorque { get; private set; }
        public float AngularVelocity { get; private set; }
        public float Compression => _compression;
        public float LongitudinalSlipVelocity { get; private set; }

        public bool IsLocked { get; private set; }
        public bool IsStopped => NearlyEqual(AngularVelocity, 0f, 1f);
        public bool IsGrounded { get; private set; }

        private Vector3 customForce;

        private RaycastHit _hit;
        private float _compression;
        private float lastLength;
        private bool isFullCompressed;
        private bool isFullUncompressed;
        private float slipAngleDynamic;
        private float hitCamberAngle;
        private new Transform transform; // Cached transform
        private float rollAngle;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetInertia()
        {
            //const int SPOKE_AMOUNT = 8;
            //const float SPOKE_MASS = 0.8f;
            //const float RIM_MASS = 3f;
            //const float LENGTH = 0.5f;

            //var sqrLength = LENGTH * LENGTH;
            //return (SPOKE_AMOUNT * (1f / 3f * SPOKE_MASS * sqrLength)) + RIM_MASS * sqrLength;

            return 1.5f;
        }

        /// <summary>
        /// Mµ
        /// </summary>
        /// <returns></returns>
        public float GetGripCoefficient()
        {
            const float DEFAULT_COEFFICIENT = 1f;
            var roadGrip = DEFAULT_COEFFICIENT;

            if (IsGrounded)
            {
                if (_hit.collider.TryGetComponent(out PhysicsSurface surface))
                    roadGrip = surface.GetFriction(IsStopped);
                else
                {
                    var material = _hit.collider.material;

                    if (material != null)
                        roadGrip = IsStopped ? material.staticFriction : material.dynamicFriction;
                }
            }

            return roadGrip * (TireFriction != null ? TireFriction.GripCurve.Evaluate(NormalForce) : 1f);
        }

        public Vector3 GetWorldSuspensionPosition()
        {
            var rideHeight = Suspension != null ? Suspension.GetRideHeight() : 0f;
            var displacement = (_compression - 1f) * (rideHeight + Radius);

            return transform == null ? base.transform.TransformPoint(0f, displacement + Radius, 0f) : transform.TransformPoint(0f, displacement + Radius, 0f);
        }

        public Vector3 GetWorldPosition() => 
            (transform == null ? base.transform.position : transform.position) + Center;

        public void ApplyTorque(float torque) => MotorTorque = torque;

        public void ApplyBrake(float input) => BrakeTorque = input * MaxBrakeTorque;

        public void ApplyForce(Vector3 force) => customForce += force;

        private enum RKStage : byte
        {
            Stage_1,
            Stage_2,
            Stage_3,
            Stage_4,
            Completed,
            Undefined
        }

        [HideInInspector] private RKStage _stage = RKStage.Undefined;
        [HideInInspector] private RKStage _nextStage = RKStage.Undefined;
        [HideInInspector] private Vector3 velo, velo0, velo1, velo2, velo3;
        [HideInInspector] private Vector3 accel1, accel2, accel3, accel4;

        private void Init()
        {
            _stage = _nextStage = RKStage.Undefined;
            _stage = RKStage.Stage_1;

            velo0 = velo;
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;

            Solver(deltaTime);
        }

        private bool Solver(float deltaTime)
        {
            var dT = deltaTime / 2f;

            switch (_stage)
            {
                case RKStage.Stage_1:
                    accel1 = Vector3.zero; // dT
                    velo1 = velo;
                    velo += accel1;
                    break;

                case RKStage.Stage_2:
                    accel2 = Vector3.zero; // dT
                    velo2 = velo;
                    velo = velo1 + accel2;
                    break;

                case RKStage.Stage_3:
                    accel3 = Vector3.zero; // dT 
                    velo3 = velo;
                    velo = velo1 + accel3;
                    break;

                case RKStage.Stage_4:
                    accel4 = Vector3.zero; // dT

                    const float V = 0.333333343f;
                    velo = velo1 + (Vector3.Scale(accel1 * 2f, accel2) + accel3 + accel4) * V;
                    break;
            }

            _nextStage = GetNextStage(_stage);
            return _nextStage == RKStage.Completed;
        }

        private void Shutdown() => _stage = _nextStage = RKStage.Undefined;

        private static RKStage GetNextStage(RKStage stage) => stage switch
        {
            RKStage.Stage_1 => RKStage.Stage_2,
            RKStage.Stage_2 => RKStage.Stage_3,
            RKStage.Stage_3 => RKStage.Stage_4,
            RKStage.Stage_4 => RKStage.Completed,
            _ => RKStage.Undefined,
        };
    }
}