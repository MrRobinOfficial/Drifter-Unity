using UnityEngine;

namespace Drifter.UIToolkit
{
    [AddComponentMenu("Tools/Drifter/UI Toolkit/Dashboard [UI Toolkit]"), DisallowMultipleComponent]
    public class DashboardUIToolkit : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] BaseVehicle m_Vehicle = default;

        private void OnEnable() { }

        private void OnDisable() { }

        private void LateUpdate() { }
    }
}