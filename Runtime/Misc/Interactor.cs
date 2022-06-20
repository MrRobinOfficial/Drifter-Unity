using UnityEngine;
using UnityEngine.InputSystem;

namespace Drifter.Misc
{
    [AddComponentMenu("Tools/Drifter/Misc/Interactor [Misc]"), DisallowMultipleComponent]
    public sealed class Interactor : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] LayerMask m_CollideWith = 1;
        [Space, SerializeField] Vector3 m_Center = default;
        [Space, SerializeField] float m_Radius = 0.5f;

        private readonly Collider[] _colliders = new Collider[3];

        private Vector3 GetWorldPosition() => transform.position + m_Center;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetWorldPosition(), m_Radius);
        }

        //private void FixedUpdate()
        //{
        //    var num = Physics.OverlapSphereNonAlloc(GetWorldPosition(),
        //        m_Radius, _colliders, m_CollideWith);

        //    var isInteracting = Keyboard.current[Key.E].wasPressedThisFrame;

        //    for (int i = 0; i < num; i++)
        //    {
        //        if (isInteracting && _colliders[i].TryGetComponent(out IInteractable interactable))
        //            interactable.TryInteract(this);
        //    }
        //}
    }
}