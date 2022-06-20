using Drifter.Extensions;
using UnityEngine;

using static Drifter.Extensions.DrifterExtensions;

namespace Drifter.Samples.Misc
{
    [AddComponentMenu("Tools/Drifter/Samples/Misc/Spinner Object [Misc]")]
    public class SpinnerObject : MonoBehaviour
    {
        [SerializeField] float m_AnglePerFrame = 1f;
        [SerializeField] AxisDirection m_Direction = AxisDirection.WorldUp;

        private Rigidbody body;

        private void Awake()
        {
            TryGetComponent(out body);
        }

        private void Update()
        {
            var axis = transform.GetAxisDirection(m_Direction);

            if (body != null)
            {
                var rot = body.rotation * Quaternion.AngleAxis(m_AnglePerFrame, axis);
                body.MoveRotation(rot);
            }
            else
            {
                var rot = transform.rotation * Quaternion.AngleAxis(m_AnglePerFrame, axis);
                transform.rotation = rot;
            }
        }
    }
}