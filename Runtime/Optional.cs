using UnityEngine;

namespace Drifter
{
    [System.Serializable]
    public class Optional<T>
    {
        [SerializeField] bool m_Enabled;
        [SerializeField] T m_Value;

        public Optional(T initialValue)
        {
            m_Enabled = true;
            m_Value = initialValue;
        }

        public bool Enabled
        {
            get => m_Enabled;
            set => m_Enabled = value;
        }

        public T Value
        {
            get => m_Value;
            set => m_Value = value;
        }

#nullable enable

        public static T? operator !(Optional<T> op) => 
            op.Enabled && op.m_Value != null ? op.m_Value : default;

#nullable disable
    }
}