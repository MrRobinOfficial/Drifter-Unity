#define ENABLE_SAVING

using Drifter.Extensions;
using Drifter.Utility;
using UnityEngine;

using static Drifter.Extensions.DrifterExtensions;
using static Drifter.Extensions.VehicleExtensions;
using static Drifter.Utility.DrifterMathUtility;

namespace Drifter
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class BaseVehicle : MonoBehaviour
    {
#if UNITY_EDITOR
        [ContextMenu("Vehicle/Toggle Rigidbody")]
        private void ToggleRigidbody() => rigidbody.hideFlags = rigidbody.hideFlags switch
        {
            HideFlags.None => HideFlags.NotEditable,
            HideFlags.NotEditable => default,
            _ => HideFlags.NotEditable,
        };

        [ContextMenu("Vehicle/Generate License Plate", isValidateFunction: false, priority: 1000100)]
        private void GenerateLicensePlate() => m_LicensePlate = VehicleExtensions.GenerateLicensePlate();

        [ContextMenu("Vehicle/Import Data", isValidateFunction: false, priority: 100050)]
        private void ImportData() => DrifterExtensions.TryImportData(this);

        [ContextMenu("Vehicle/Export Data", isValidateFunction: false, priority: 100050)]
        private void ExportData() => DrifterExtensions.TryExportData(this);

        [ContextMenu("Vehicle/Enable Saving", isValidateFunction: true, priority: 100150)]
        private bool ToggleSaving() => !_autoSave;

        [ContextMenu("Vehicle/Disable Saving", isValidateFunction: true, priority: 100150)]
        private bool DisableSaving() => _autoSave;

        [ContextMenu("Vehicle/Enable Saving", isValidateFunction: false, priority: 100150)]
        private void EnableSavingFunc() => _autoSave = true;

        [ContextMenu("Vehicle/Disable Saving", isValidateFunction: false, priority: 100150)]
        private void DisableSavingFunc() => _autoSave = false;

        [HideInInspector] private bool _autoSave = false;
#endif

        [SerializeField, HideInInspector] private string m_SubPath = DrifterExtensions.DEFAULT_FILE_PATH;

        [SerializeField, HideInInspector] private string m_LicensePlate = string.Empty;

        /// <summary>
        /// Helpful for SDK
        /// </summary>
        public string LicensePlate => m_LicensePlate;

        /// <summary>
        /// Path for loading/saving<br></br>
        /// <b>NOTE:</b> Directory is always <see cref="Application.streamingAssetsPath"/> 
        /// </summary>
        public string SubPath => m_SubPath;

        public new string name = "Mazda Rx7";

        /// <summary>
        /// In [kg]
        /// </summary>
        [field: Header("Vehicle Settings")]
        [field: SerializeField] public float Mass { get; set; } = 1500f;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField] public Optional<Vector3> CenterOfMass { get; set; } = default;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField] public Optional<Vector3> Inertia { get; set; } = default;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField] public float DragCoefficient { get; set; } = 0.3f;

        /// <summary>
        /// Negative value = down force
        /// </summary>
        [field: SerializeField] public float LiftCoefficient { get; set; } = -1f;

        /// <summary>
        /// In [m^2]
        /// </summary>
        [field: SerializeField] public float FrontalArea { get; set; } = 1.94f;

        public bool IsDriveable { get; set; } = true;

        /// <summary>
        /// Chassis weight<br></br>
        /// In [kg]
        /// </summary>
        public float CurbWeight => Mass;

        /// <summary>
        /// Chassis weight + fuel tank weight + driver weight<br></br>
        /// In [kg]
        /// </summary>
        public float GrossWeight { get; protected set; }

        /// <summary>
        /// In [km]
        /// </summary>
        public float Distance { get; private set; }

        /// <summary>
        /// Helpful for LOD system
        /// </summary>
        public bool IsVisible { get; protected set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 GForce { get; private set; } = default;

        public Vector2 GetGForceNormalized()
        {
            var combinedSlip = new Vector2(GForce.x, GForce.z);
            return combinedSlip.magnitude > 1f ? combinedSlip.normalized : combinedSlip;
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Acceleration { get; private set; } = Vector3.zero;

        /// <summary>
        /// In [km/h]
        /// </summary>
        public InterpolatedFloat Speedometer { get; protected set; } = new();

        /// <summary>
        /// In [km]
        /// </summary>
        public InterpolatedFloat Odometer { get; protected set; } = new();

        /// <summary>
        /// In [RPM]
        /// </summary>
        public InterpolatedFloat Tachometer { get; protected set; } = new();

        /// <summary>
        /// In [m/s]
        /// </summary>
        public float RightVelocity { get; private set; }

        /// <summary>
        /// In [m/s]
        /// </summary>
        public float ForwardVelocity { get; private set; }

        /// <summary>
        /// Combined right and forward velocity<br></br>
        /// In [m/s]
        /// </summary>
        public float LinearVelocity { get; private set; }
        public float GetSpeedInKph() => Mathf.Round(Mathf.Abs(ForwardVelocity) * 3.6f);

        public Rigidbody Body
        {
            get
            {
                if (rigidbody == null)
                    rigidbody = GetComponent<Rigidbody>();

                return rigidbody;
            }
        }

        protected new Transform transform;
        protected new Rigidbody rigidbody;
        private Vector3 oldPosition;

        private Vector3 spawnPosition;
        private Vector3 spawnRotation;

        public void SetSpawnLocation(Vector3 position, Quaternion rotation)
        {
            spawnPosition = position;
            spawnRotation = rotation.eulerAngles;
        }

        [ContextMenu("Vehicle/Respawn")]
        public void Respawn()
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.Sleep();

            var location = spawnPosition + Vector3.up;
            rigidbody.Move(location, Quaternion.Euler(spawnRotation));
        }

        protected virtual void OnValidate()
        {
            //if (rigidbody == null)
            //{
            //    if (!TryGetComponent(out rigidbody))
            //        rigidbody = gameObject.AddComponent<Rigidbody>();
            //}

            //rigidbody.mass = Mass;
            //rigidbody.drag = 0f;
            //rigidbody.angularDrag = 0f;
            //rigidbody.useGravity = true;
            //rigidbody.isKinematic = false;
            //rigidbody.freezeRotation = false;
            //rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            //rigidbody.hideFlags = HideFlags.NotEditable;
        }

        protected virtual void OnDrawGizmos() { }

        protected virtual void OnDrawGizmosSelected() { }

        private void Awake()
        {
#if UNITY_EDITOR
            if (_autoSave)
                DrifterExtensions.TryImportData(this);
#else
            DrifterExtensions.TryImportData(this);
#endif

            transform = base.transform;
            rigidbody = GetComponent<Rigidbody>();
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (_autoSave)
                DrifterExtensions.TryExportData(this);
#else
            DrifterExtensions.TryExportData(this);
#endif
        }

        private void OnEnable()
        {
            rigidbody.mass = Mass;

            if (CenterOfMass.Enabled)
                rigidbody.centerOfMass = CenterOfMass.Value;
            else
                rigidbody.ResetCenterOfMass();

            if (Inertia.Enabled)
                rigidbody.inertiaTensor = Inertia.Value;
            else
                rigidbody.ResetInertiaTensor();

            OnInit();
        }

        private void OnDisable() => OnShutdown();

        private void Update()
        {
            var deltaTime = Time.deltaTime;

            var localVelocity = transform.InverseTransformDirection(Body.velocity);

            RightVelocity = localVelocity.x;
            ForwardVelocity = localVelocity.z;
            LinearVelocity = new Vector2(localVelocity.x, localVelocity.z).magnitude;

            //Speedometer = Mathf.SmoothDamp(Speedometer, Mathf.Abs(speedInKph), ref _currentSpeedInKph, smoothDeltaTime * GAUGE_SPEED);

            OnSimulate(deltaTime);
        }

        private void FixedUpdate()
        {
            Distance = DistanceXZ(transform.position, oldPosition);

            var deltaTime = Time.fixedDeltaTime;

            Body.AddForce(transform.TransformDirection(this.CalcAerodynamics()));

            OnFixedSimulate(deltaTime);

            oldPosition = transform.position;
        }

        #region Callbacks

        /// <summary>
        /// Collect references and initialize other modules
        /// </summary>
        protected abstract void OnInit();

        /// <summary>
        /// Update method for <b>VISUAL OBJECTS ONLY</b>
        /// </summary>
        /// <param name="deltaTime"></param>
        protected abstract void OnSimulate(float deltaTime);

        /// <summary>
        /// Update method for modules
        /// </summary>
        /// <param name="deltaTime"></param>
        protected abstract void OnFixedSimulate(float deltaTime);

        /// <summary>
        /// De-subscribe events and shutdown other modules
        /// </summary>
        protected abstract void OnShutdown();

        #endregion

        #region Data Saving

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public virtual void LoadData(FileData data)
        {
            data.ReadValue("Vehicle", "LicensePlate", out m_LicensePlate);
            data.ReadValue("Vehicle", "DisplayName", out string displayName);

            Print($"{displayName} has been loaded!", PrintType.Log);

            data.ReadValue("Vehicle", "Mass", out float mass);

            Mass = mass;

            data.ReadValue("Vehicle", "OverrideCenterOfMass", out bool overrideCenterOfMass);
            data.ReadValue("Vehicle", "CenterOfMass", out Vector3 centerOfMass);

            data.ReadValue("Vehicle", "OverrideInertia", out bool overrideInertia);
            data.ReadValue("Vehicle", "Inertia", out Vector3 inertia);

            CenterOfMass.Enabled = overrideCenterOfMass;
            CenterOfMass.Value = centerOfMass;

            Inertia.Enabled = overrideInertia;
            Inertia.Value = inertia;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual FileData SaveData()
        {
            var data = new FileData();

            data.WriteValue("Vehicle", "LicensePlate", LicensePlate);
            data.WriteValue("Vehicle", "DisplayName", "Mazda Rx7");

            data.WriteValue("Vehicle", "Mass", Mass);

            data.WriteValue("Vehicle", "OverrideCenterOfMass", CenterOfMass.Enabled);
            data.WriteValue("Vehicle", "CenterOfMass", CenterOfMass.Value);

            data.WriteValue("Vehicle", "OverrideInertia", Inertia.Enabled);
            data.WriteValue("Vehicle", "Inertia", Inertia.Value);

            return data;
        } 

        #endregion
    } 
}