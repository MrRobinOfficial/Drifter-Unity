using UnityEngine;
using UnityEngine.VFX;

namespace Drifter.Handlers
{
    [RequireComponent(typeof(AudioSource))]
    public class ExhaustHandler : BaseHandler
    {
        private ExhaustData data;

        public readonly struct ExhaustData
        {
            public readonly VisualEffectAsset flameAsset;
            public readonly AudioClip loopClip;
        }

        private VisualEffect flameVFX;

        public void Init(ExhaustData data)
        {
            this.data = data;

            if (data.flameAsset != null)
            {
                flameVFX = gameObject.AddComponent<VisualEffect>();
                flameVFX.visualEffectAsset = data.flameAsset;
            }
        }

        private void OnEnable()
        {
            if (flameVFX != null)
                flameVFX.Stop();
        }

        private void Update()
        {
            const int BUTTON_INDEX = 0;

            if (flameVFX != null && Input.GetMouseButtonDown(BUTTON_INDEX))
                flameVFX.Play();

            if (flameVFX != null && Input.GetMouseButtonUp(BUTTON_INDEX))
                flameVFX.Stop();
        }
    } 
}