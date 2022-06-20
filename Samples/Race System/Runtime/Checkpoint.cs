using UnityEngine;

namespace Drifter.Samples.RaceSystem
{
    [RequireComponent(typeof(Collider))]
	public sealed class Checkpoint : MonoBehaviour
	{
        private new Collider collider;

        public int Index { get; private set; } = 0;

        public bool IsFinale => Index == 0;

        public void Init(int index)
        {
            collider = GetComponent<Collider>();
            collider.isTrigger = true;

            Index = index;
        }

        private void Start() => hideFlags = HideFlags.NotEditable;

        private void OnTriggerEnter(Collider other)
        {
            var root = other.transform.root;

            if (!root.TryGetComponent(out Racer racer))
                return;

            racer.AddCheckpoint(this);
        }

        private void OnTriggerExit(Collider other)
        {

        }
    }
}