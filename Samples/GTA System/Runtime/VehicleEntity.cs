using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Drifter.Samples.GTASystem
{
    [AddComponentMenu("Tools/Drifter/Samples/GTA System/Vehicle [Entity]"), DisallowMultipleComponent]
    [RequireComponent(typeof(BaseVehicle))]
    [RequireComponent(typeof(AudioSource))]
    public class VehicleEntity : MonoBehaviour
    {
        public event UnityAction<Object, short> OnDamageTaken;

        public int Health { get; private set; } = 0;

        public float GetHealthNormalized() => Application.isPlaying ? (float)Health / m_StartingHealth : 1f;

        public bool IsAlive => Health > 0;

        [Header("General Settings")]
        [SerializeField] short m_StartingHealth = 1000;

        [Header("VFX")]
        [SerializeField] GameObject m_ExplodeVFX = default;
        [SerializeField] GameObject m_FireVFX = default;

        [Header("SFX")]
        [SerializeField] AudioClip m_ExplodeSFX = default;

        private BaseVehicle vehicle;
        private AudioSource audioSource;

        private void Awake()
        {
            vehicle = GetComponent<BaseVehicle>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start() => Health = m_StartingHealth;

        private void OnCollisionEnter(Collision collision)
        {
            print($"{collision.impulse:N}");
        }

        public void Destroy() => TakeDamage(sender: null, (short)Health);

        public void TakeDamage(Object sender, short amount)
        {
            if (!Application.isPlaying)
                return;

            OnDamageTaken?.Invoke(sender, amount);
            Health -= amount;

            if (Health < 0)
            {
                Health = 0;
                OnDeath();
            }
        }

        public void Repair(short amount)
        {
            Health += amount;
            Health = Mathf.Min(Health, m_StartingHealth);
        }

        #region Callbacks
        [ContextMenu("Callbacks/OnDeath")]
        protected virtual void OnDeath()
        {
            Health = 0;

            var explosionForce = 4000f;
            var blastRadius = 10f; // vehicle.Size.sqrMagnitude;

            var colliders = Physics.OverlapSphere(transform.position, blastRadius);

            foreach (var collider in colliders)
            {
                if (collider.attachedRigidbody == null)
                    continue;

                collider.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, blastRadius);
            }

            TrySpawnVFX(m_ExplodeVFX, destroyTime: 3f);
            TrySpawnVFX(m_FireVFX, destroyTime: 3f);
            TryPlaySFX(m_ExplodeSFX);

            vehicle.IsDriveable = false;
        }
        #endregion

        private void TrySpawnVFX(GameObject prefab, float destroyTime)
        {
            if (prefab == null)
                return;

            var obj = Instantiate(prefab, transform.position, transform.rotation);
            Destroy(obj, destroyTime);
        }

        private void TryPlaySFX(AudioClip sfx, float volumeScale = 1f)
        {
            if (sfx == null)
                return;

            audioSource.PlayOneShot(sfx, volumeScale);
        }
    }
}