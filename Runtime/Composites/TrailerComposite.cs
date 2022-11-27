using UnityEngine;

namespace Drifter.Composites
{
    [AddComponentMenu("Tools/Drifter/Composites/Trailer [Composite]"), DisallowMultipleComponent]
    public class TrailerComposite : MonoBehaviour, IHookable
    {
        public HingeJoint Connection { get; private set; } = null;

        [field: SerializeField] public uint Weight { get; set; } = 500;

        public void Connect(Transform transform, bool overrideConnection)
        {
            if (Connection != null && !overrideConnection)
                return;

            Connection = transform.TryGetComponent(out HingeJoint joint) ? 
                joint : transform.gameObject.AddComponent<HingeJoint>();
        }

        public void Disconnect()
        {
            if (Connection == null)
                return;

            Destroy(Connection);
        }
    }
}