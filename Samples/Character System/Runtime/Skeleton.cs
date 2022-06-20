using UnityEngine;
using UnityEngine.Events;

namespace Drifter.Samples.CharacterSystem
{
	public class Skeleton : MonoBehaviour
	{
        private UnityAction onVisible;
        private UnityAction onInvisible;

        private void Awake() => hideFlags = HideFlags.NotEditable;

        public void Init(UnityAction onVisible, UnityAction onInvisible)
        {
            this.onVisible = onVisible;
            this.onInvisible = onInvisible;
        }

        private void OnBecameVisible() => onVisible?.Invoke();

        private void OnBecameInvisible() => onInvisible?.Invoke();
    }
}