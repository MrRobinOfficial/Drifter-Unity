#define DRAW_RAYCAST
#undef DRAW_RAYCAST

using UnityEngine;
using Drifter.Data;
using Drifter.Misc;
using Drifter.Attributes;

using static Drifter.Extensions.DrifterExtensions;
using static Drifter.Extensions.WheelExtensions;
using static Drifter.Utility.DrifterMathUtility;
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

        [field: SerializeField] public RaycastCastType RaycastCast { get; set; } = RaycastCastType.Single;
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

        public Vector3 GetWorldPosition() => (transform == null ? base.transform.position : transform.position) + Center;

        public void ApplyTorque(float torque) => MotorTorque = torque;

        public void ApplyBrake(float input) => BrakeTorque = input * MaxBrakeTorque;

        public void ApplyForce(Vector3 force) => customForce += force;

        private void Start()
        {
            transform = base.transform;

            if (Vehicle == null)
            {
                enabled = false;
                throw new System.NullReferenceException("Missing Vehicle component!");
            }

            if (Visual != null)
                Visual.SetParent(transform);
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;

            if (Vehicle != null)
            {
                //rotOffset.Set(casterAngle, toeAngle, camberAngle);
                //rotOffset.Scale(m_ScaleOffset);

                transform.localRotation = Quaternion.AngleAxis(SteerAngle, Vector3.up);
            }

            if (Visual != null)
            {
                //m_Visual.position = GetWorldPosition();

                var rideHeight = Suspension != null ? Suspension.GetRideHeight() : 0f;
                Visual.localPosition = (_compression - 1f) * rideHeight * Vector3.up;

                var angle = Mathf.Rad2Deg * AngularVelocity;
                Visual.Rotate(transform.right, angle * deltaTime, Space.World);
            }
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;

            if (Vehicle != null && Vehicle.Body != null)
                Vehicle.Body.AddForceAtPosition(Physics.gravity * Mass, transform.position);

            var ray = new Ray
            {
                origin = transform.position + transform.up * Radius,
                direction = -transform.up,
            };

            //const float GROUND_PENETRATION = 0f; 
            //IsGrounded = Physics.Raycast(ray, out _hit, rideHeight + Radius, CollideWith);

            IsGrounded = this.WheelCast(RaycastCast, ray, out _hit, CollideWith);

            //IsGrounded = this.WheelCast(Radius, Width, transform.position, Vector3.up, transform.forward, transform.forward, out _hit, CollideWith);

            var slopeAngle = Vector3.Angle(_hit.normal, transform.up);

            if (Mathf.Abs(slopeAngle) > SLOPE_THRESHOLD)
                IsGrounded = false;

            if (!IsGrounded)
            {
                LongitudinalForce = LateralForce = AligningTorque = NormalForce = 0f;
                SlipRatio = SlipAngle = 0f;
            }

            var collisionPoint = IsGrounded ? _hit.point : transform.position;

            if (Vehicle != null && Vehicle.Body != null)
            {
                var targetVelo = Vector3.zero;

                if (_hit.rigidbody != null)
                    targetVelo = _hit.rigidbody.GetPointVelocity(collisionPoint);

                var relativeVelo = Vehicle.Body.GetPointVelocity(collisionPoint) - targetVelo;
                VelocityAtWheel = transform.InverseTransformDirection(relativeVelo);
            }

#if DRAW_RAYCAST
            Debug.DrawLine(ray.origin, collisionPoint, IsGrounded ? Color.red : Color.green);
#endif

            var fwd = transform.forward;
            fwd.y = 0f;
            fwd *= Mathf.Sign(transform.up.y);
            var right = Vector3.Cross(Vector3.up, fwd).normalized;
            rollAngle = Vector3.Angle(right, transform.right) * Mathf.Sign(transform.right.y);

            var v = Mathf.Cos(rollAngle * Mathf.Deg2Rad);

            NormalForce = Suspension.CalcSuspension(this, deltaTime, ref _compression, ref lastLength, ref isFullCompressed, ref isFullUncompressed) * v;

            var grip = GetGripCoefficient();
            var inertia = GetInertia();

            var hitDot = Mathf.Clamp(Vector3.Dot(_hit.normal, transform.right), -1f, 1f);
            hitCamberAngle = Mathf.Rad2Deg * ((Mathf.PI / 2f) - Mathf.Acos(hitDot));

            var tractionForce = Mathf.Min(LongitudinalForce, NormalForce * grip);
            TractionTorque = tractionForce * Radius;

            AngularVelocity += (MotorTorque - TractionTorque) / inertia * deltaTime;

            var prevSign = Sign(AngularVelocity);
            var rollResistanceTorque = NormalForce * Radius * RollingResistance;
            AngularVelocity += (BrakeTorque + rollResistanceTorque) * -prevSign / inertia * deltaTime;

            IsLocked = prevSign != Sign(AngularVelocity);
            AngularVelocity = IsLocked ? 0f : AngularVelocity;  // Zero cross check
            LongitudinalSlipVelocity = (AngularVelocity * Radius) - VelocityAtWheel.z;

            SlipRatio = GetSx(deltaTime, this);
            CalcSlipAngle();

            //SlipAngle = GetSlipAngle(VelocityAtWheel);

            //SlipAngle = GetSy(deltaTime, ref slipAngleDynamic, VelocityAtWheel); 
            //GetSy(deltaTime, ref slipAngleDynamic, VelocityAtWheel);

            if (TireFriction != null)
            {
                var roadForces = grip * TireFriction.GetCombinedForce(NormalForce, SlipRatio, SlipAngle);

                LateralForce = roadForces.x;
                AligningTorque = roadForces.y;
                LongitudinalForce = roadForces.z;
            }

            if (IsGrounded && Vehicle != null && Vehicle.Body != null)
            {
                //var dot = Vector3.Dot(transform.position, _hit.point);

                //Debug.Log(dot);

                var forces = new Vector3(LateralForce, NormalForce, LongitudinalForce);
                forces += customForce;
                Vehicle.Body.AddForceAtPosition(transform.TransformDirection(forces), _hit.point);

                customForce = Vector3.zero;
            }

            void CalcSlipAngle()
            {
                // Atan = radians

                var highSlipAngle = 0f;
                var velocityAtWheel = VelocityAtWheel;
                velocityAtWheel.y = 0f;

                if (velocityAtWheel.z != 0f)
                    highSlipAngle = Mathf.Rad2Deg * Mathf.Atan(-velocityAtWheel.x / Mathf.Abs(velocityAtWheel.z));

                const float SLIP_ANGLE_PEAK = 8f; // [deg]
                const float RELAXATION_LENGTH = 0.005f;
                const float LOW_SPEED = 3f; // [m/s]
                const float HIGH_SPEED = 6f; // [m/s]

                var lowSlipAngle = SLIP_ANGLE_PEAK * Sign(-velocityAtWheel.x);

                var t = RemapClamped(velocityAtWheel.magnitude, LOW_SPEED, HIGH_SPEED, 
                    outA: 0f, outB: 1f);

                var slip = Mathf.Lerp(lowSlipAngle, highSlipAngle, t); // Lerp between low speed slip angle and high speed slip angle

                //var coeff = Mathf.Clamp01(Mathf.Abs(velocityAtWheel.x) / RELAXATION_LENGTH * deltaTime);

                slipAngleDynamic += (slip - slipAngleDynamic) * 1f;
                slipAngleDynamic = Mathf.Clamp(slipAngleDynamic, min: -90f, max: 90f);

                SlipAngle = slipAngleDynamic / SLIP_ANGLE_PEAK;
                //Mathf.Clamp(slipAngleDynamic / SLIP_ANGLE_PEAK, min: -1f, max: 1f);
            }
        }
    }
}