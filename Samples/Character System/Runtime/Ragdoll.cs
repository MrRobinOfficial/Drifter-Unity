using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Drifter.Samples.CharacterSystem
{
	public class Ragdoll : MonoBehaviour
	{
        public event UnityAction OnActivated;
        public event UnityAction OnDeactivated;

        private Bone[] _bones = default;

        [Header("References")]
        [SerializeField] Renderer m_Skeleton = default;

        [Header("General Settings")]
        [SerializeField] float m_Thickness = 0.3f;
        [SerializeField] bool m_ActivateOnCollision = true;
        [SerializeField, ShowIf(nameof(m_ActivateOnCollision)), Tag] string m_IgnoreCollisionTag = string.Empty;
        [SerializeField, ShowIf(nameof(m_ActivateOnCollision))] LayerMask m_CollisionLayer = default;
        [SerializeField, ShowIf(nameof(m_ActivateOnCollision))] float m_CollisionThreshold = 10f;

        [Header("Events")]
        [SerializeField] UnityEvent m_OnActivated;
        [SerializeField] UnityEvent m_OnDeactivated;

        private void Awake()
        {
            if (m_Skeleton != null)
            {
                var obj = m_Skeleton.gameObject.AddComponent<Skeleton>();
                obj.Init(EnableBones, DisableBones);
            }

            var rigidbodies = GetComponentsInChildren<Rigidbody>(includeInactive: true);

            _bones = new Bone[rigidbodies.Length];

            for (int i = 0; i < rigidbodies.Length; i++)
            {
                var bone = rigidbodies[i].gameObject.AddComponent<Bone>();
                bone.Init(OnBoneCollide, m_Thickness);
                _bones[i] = bone;
            }
        }

        [ContextMenu("Ragdoll/Enable")]
        public void EnableRagdoll()
        {
            for (var i = 0; i < _bones.Length; i++)
                _bones[i].EnableRigidbody();

            if (TryGetComponent(out Animator animator))
                animator.enabled = false;

            m_OnActivated?.Invoke();
            OnActivated?.Invoke();
        }

        [ContextMenu("Ragdoll/Disable")]
        public void DisableRagdoll()
        {
            for (var i = 0; i < _bones.Length; i++)
                _bones[i].DisableRigidbody();

            if (TryGetComponent(out Animator animator))
                animator.enabled = true;

            m_OnDeactivated?.Invoke();
            OnDeactivated?.Invoke();
        }

        public void EnableBones()
        {
            for (int i = 0; i < _bones.Length; i++)
                _bones[i].enabled = true;

            enabled = true;
        }

        public void DisableBones()
        {
            for (int i = 0; i < _bones.Length; i++)
                _bones[i].enabled = false;

            enabled = false;
        }

        private void OnBoneCollide(Bone bone, Collision collision)
        {
            if (collision.gameObject == null ||
                collision.relativeVelocity.magnitude < m_CollisionThreshold ||
                !IsInLayerMask(collision.gameObject) ||
                (!string.IsNullOrEmpty(m_IgnoreCollisionTag) && collision.gameObject.CompareTag(m_IgnoreCollisionTag)))
                return;

            EnableRagdoll();
        }

        private bool IsInLayerMask(GameObject obj) => (m_CollisionLayer.value & (1 << obj.layer)) > 0;
    }
}