using UnityEngine;

namespace Drifter.Misc
{
    [AddComponentMenu("Tools/Drifter/Misc/Stabilizer [Misc]"), DisallowMultipleComponent]
    public class Stabilizer : MonoBehaviour
    {
        [Header("General Config")]
        [SerializeField] float m_DampenFactor = 0.8f;
        [SerializeField] float m_AdjustFactor = 0.5f;

        private Rigidbody body = null;
        private new Transform transform = null;

        private void Awake()
        {
            transform = base.transform;
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            var deltaQuat = Quaternion.FromToRotation(transform.up, Vector3.up);
            deltaQuat.ToAngleAxis(out var angle, out var axis);

            body.AddTorque(-body.angularVelocity * m_DampenFactor, ForceMode.Acceleration);
            body.AddTorque(axis.normalized * angle * m_AdjustFactor, ForceMode.Acceleration);
        }
    }
}
