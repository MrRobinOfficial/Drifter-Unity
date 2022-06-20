using UnityEngine;

namespace Drifter.Composites
{
    public abstract class BaseCameraComposite : BaseCarComposite
    {
        [Header("General Settings")]
        [SerializeField] CameraType m_Type;

        public CameraType Type
        {
            get => m_Type;
            set
            {
                m_Type = value;
                OnViewSwitched(m_Type);
            }
        }

        protected abstract void OnViewSwitched(CameraType type);
    }
}