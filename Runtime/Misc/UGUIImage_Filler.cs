using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Drifter.Misc
{
    [AddComponentMenu("Tools/Drifter/Misc/UGUI Image Filler [Misc]"), DisallowMultipleComponent]
    [ExecuteAlways, RequireComponent(typeof(Image))]
    public class UGUIImage_Filler : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] bool m_EnableLerp = true;
        [SerializeField, ShowIf(nameof(m_EnableLerp))] float m_LerpMulti = 20f;
        [SerializeField] float m_FillOffset = 1f;
        [SerializeField, Range(0f, 1f)] float m_FillAmount = 1f;

        public float FillAmount
        {
            get => m_FillAmount;
            set => m_FillAmount = Mathf.Clamp01(value);
        }

        private Image image;

        private void Awake() => image = GetComponent<Image>();

        private void Update() => image.fillAmount = m_EnableLerp
                ? Mathf.Lerp(image.fillAmount, FillAmount * m_FillOffset, m_LerpMulti * Time.deltaTime)
                : FillAmount * m_FillOffset;
    } 
}