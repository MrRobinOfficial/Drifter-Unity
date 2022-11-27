using Drifter.Extensions;
using Drifter.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Drifter
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class BaseVehicle : MonoBehaviour, ISerializationCallbackReceiver
    {
        //public static event UnityAction<Collision> OnCollided;

        protected const string k_FileName = "Vehicle.ini";

        [SerializeField, HideInInspector] string m_ID = string.Empty;
        [SerializeField, HideInInspector] string m_DirectoryPath = string.Empty;

        [field: SerializeField] public string DisplayName { get; protected set; } = string.Empty;
        [field: SerializeField] public float Mass { get; protected set; } = 1200f;
        [field: SerializeField] public Vector3 Inertia { get; protected set; } = Vector3.one;
        [field: SerializeField] public float DragCoefficient { get; protected set; } = 0.35f;
        [field: SerializeField] public float LiftCoefficient { get; protected set; } = -1f;

        public Vector3 GForce { get; private set; } = Vector3.zero;

        public string ID => m_ID;
        public string DirectoryPath => m_DirectoryPath;
        public Rigidbody Rigidbody
        {
            get
            {
                if (rigidbody == null)
                {
                    rigidbody = GetComponent<Rigidbody>();
                    rigidbody.hideFlags = HideFlags.NotEditable;
                    rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                    rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    rigidbody.drag = rigidbody.angularDrag = 0f;
                    rigidbody.solverIterations = Physics.defaultSolverIterations;
                    rigidbody.solverVelocityIterations = Physics.defaultSolverVelocityIterations;
                    rigidbody.useGravity = true;
                    rigidbody.isKinematic = false;
                }

                return rigidbody;
            }
        }

        //public bool IsColliding { get; private set; } = false;

        //private void OnCollisionEnter(Collision collision) { }

        //private void OnCollisionStay(Collision collision) => IsColliding = true;

        //private void OnCollisionExit(Collision collision) => IsColliding = false;

        public Vector3 GetLocalVelocity() => transform.InverseTransformDirection(Rigidbody.velocity);

        public float GetSpeedInKph() => MathUtility.ConvertMsToKph(GetLocalVelocity().z);

        public float HeadingAngle { get; private set; } = 0f;

        public float GetHeadingAngleNormalized() => HeadingAngle / 90f;

        private new Rigidbody rigidbody;
        private new Transform transform;

        //public DataBus DataBus = new(); // Temporary

        protected virtual void Awake()
        {
            transform = base.transform;

            rigidbody = GetComponent<Rigidbody>();
            rigidbody.hideFlags = HideFlags.NotEditable;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody.drag = rigidbody.angularDrag = 0f;
            rigidbody.solverIterations = Physics.defaultSolverIterations;
            rigidbody.solverVelocityIterations = Physics.defaultSolverVelocityIterations;
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
        }

        protected virtual void OnEnable()
        {
            this.RegisterVehicle();
            FetchData();
        }

        protected virtual void OnDisable()
        {
            this.UnregisterVehicle();
            PushData();
        }

        protected virtual void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;

            HeadingAngle = rigidbody.velocity.magnitude > 10f ? 
                Vector3.SignedAngle(rigidbody.velocity, transform.forward, transform.up) : 0f;

            var velo = GetLocalVelocity();
            GForce = (velo - lastFrameVelocity) / (deltaTime * Physics.gravity.y);
            lastFrameVelocity = velo;
        }

        private Vector3 lastFrameVelocity;

        /// <summary>
        /// Loading vehicle data
        /// </summary>
        public virtual void FetchData()
        {
            if (!IsDirectoryValid())
            {
#if DEBUG
                Debug.LogWarning(message: "Fetching data resulting in error! File path does not exist at the moment!", this);
#endif

                return;
            }

            if (!this.TryFetchData(k_FileName, out var data))
                return;

            Load(data);
        }

        /// <summary>
        /// Saving vehicle data
        /// </summary>
        public virtual void PushData()
        {
            if (!IsDirectoryValid())
            {
#if DEBUG
                Debug.LogWarning(message: "Pushing data resulting in error! File path does not exist at the moment!", this);
#endif

                return;
            }

            this.TryPushData(k_FileName, Save());
        }

        #region Data Saving

        [ContextMenu("Reset ID", isValidateFunction: false, priority: 1000050)]
        protected void ResetID()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorUtility.DisplayDialog(title: "Reseting ID",
                message: $"Are you sure you want to reset current ID of your vehicle?\n\nThis WILL lose all connection to folder structure of .INI files.\n\nWhich is located at '{DrifterExtensions.GetSubDirectory()}'",
                ok: "Yes", cancel: "Cancel"))
                return;
#endif

            m_ID = string.Empty;
        }

        [ContextMenu("Copy ID", isValidateFunction: false, priority: 1000050)]
        protected void CopyID()
        {
            GUIUtility.systemCopyBuffer = ID;

#if UNITY_EDITOR
            foreach (UnityEditor.SceneView scene in UnityEditor.SceneView.sceneViews)
                scene.ShowNotification(new GUIContent(text: "Copy to clipboard"));
#endif
        }

        public bool IsDirectoryValid()
        {
            if (string.IsNullOrEmpty(m_DirectoryPath))
                this.IsDirectoryValid(out m_DirectoryPath);

            if (System.IO.Directory.Exists(m_DirectoryPath))
                return true;
            else
            {
                m_DirectoryPath = string.Empty;
                return false;
            }
        }

        protected virtual FileData Save()
        {
            var data = new FileData();

            data.WriteValue("General", "DisplayName", DisplayName);
            data.WriteValue("General", "Mass", rigidbody.mass);

            (bool Enabled, Vector3 Value) CenterOfMass = default;

            data.WriteValue("General", "OverrideCenterOfMass", CenterOfMass.Enabled);
            data.WriteValue("General", "CenterOfMass", CenterOfMass.Value);

            (bool Enabled, Vector3 Value) Inertia = default;

            data.WriteValue("General", "OverrideInertia", Inertia.Enabled);
            data.WriteValue("General", "Inertia", Inertia.Value);

            return data;
        }

        protected virtual void Load(FileData data)
        {
            data.ReadValue("General", "DisplayName", out string displayName);
            DisplayName = displayName;

            data.ReadValue("General", "Mass", out float mass);

            data.ReadValue("General", "OverrideCenterOfMass", out bool overrideCOM);
            data.ReadValue("General", "CenterOfMass", out Vector3 COM);

            data.ReadValue("General", "OverrideInertia", out bool overrideInertia);
            data.ReadValue("General", "Inertia", out Vector3 inertia);
        } 
        #endregion

        #region Callbacks
        public void OnBeforeSerialize()
        {
            if (!string.IsNullOrEmpty(m_ID))
                return;

            m_ID = System.Guid.NewGuid().ToString();
        }

        public void OnAfterDeserialize() { }
        #endregion
    }
}