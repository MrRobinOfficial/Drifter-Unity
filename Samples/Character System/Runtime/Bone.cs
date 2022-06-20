using UnityEngine;
using UnityEngine.Events;

namespace Drifter.Samples.CharacterSystem
{
	public class Bone : MonoBehaviour
	{
        private const RigidbodyInterpolation INTERPOLATION = RigidbodyInterpolation.Interpolate;
        private const CollisionDetectionMode COLLISION_DETECTION_MODE = CollisionDetectionMode.Continuous;

        private new Rigidbody rigidbody;
        private Collider[] colliders;

        private UnityAction<Bone, Collision> callback;

        private void Awake()
        {
            hideFlags = HideFlags.NotEditable;
            rigidbody = GetComponent<Rigidbody>();
            colliders = GetComponents<Collider>();

            rigidbody.isKinematic = true;
            rigidbody.interpolation = INTERPOLATION;
            rigidbody.collisionDetectionMode = COLLISION_DETECTION_MODE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="contactOffset">Ranges from 0.3 -> 0.5</param>
        public void Init(UnityAction<Bone, Collision> callback, float contactOffset = 0.5f)
        {
            this.callback = callback;

            for (int i = 0; i < colliders.Length; i++)
                colliders[i].contactOffset += contactOffset;

            DisableRigidbody();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == null)
                return;

            callback?.Invoke(this, collision);
        }

        private Vector3 savedVelocity;
        private Vector3 savedAngularVelocity;

        public void EnableRigidbody()
        {
            rigidbody.velocity = savedVelocity;
            rigidbody.angularVelocity = savedAngularVelocity;

            rigidbody.detectCollisions = true;
            rigidbody.isKinematic = false;
            rigidbody.WakeUp();
        }

        public void DisableRigidbody()
        {
            savedVelocity = rigidbody.velocity;
            savedAngularVelocity = rigidbody.angularVelocity;

            rigidbody.detectCollisions = false;
            rigidbody.isKinematic = true;
            rigidbody.Sleep();
        }

        private void OnEnable()
        {
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].enabled = true;

            EnableRigidbody();
        }

        private void OnDisable()
        {
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].enabled = false;

            DisableRigidbody();
        }
    }
}