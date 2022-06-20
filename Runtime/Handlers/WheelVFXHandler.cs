using Drifter.Components;
using UnityEngine;

namespace Drifter.Handlers
{
    [RequireComponent(typeof(WheelBehaviour))]
	public class WheelVFXHandler : MonoBehaviour
	{
        private ParticleSystem smokeVFX;
        private TrailRenderer skidVFX;

        private WheelBehaviour wheel;
        private bool isLocked;

        public void Init(TrailRenderer skidVFX, ParticleSystem smokeVFX)
        {
            this.skidVFX = Instantiate(skidVFX);

            this.smokeVFX = Instantiate(smokeVFX, transform);
            this.smokeVFX.Stop();

            this.skidVFX.widthMultiplier = wheel.Width;
        }

        private void Awake()
        {
            hideFlags = HideFlags.NotEditable;

            wheel = GetComponent<WheelBehaviour>();
        }

        private void Update()
        {
            HandleSmoking();
            HandleSkidding();
        }

        private void HandleSmoking()
        {
            //if (!wheel.IsGrounded && !wheel.IsLocked)
            //{
            //    if (smokeVFX != null && smokeVFX.isPlaying)
            //        smokeVFX.Stop();

            //    return;
            //}

            //if (smokeVFX != null)
            //{
            //    if (!smokeVFX.isPlaying)
            //        smokeVFX.Play();

            //    var emission = smokeVFX.emission;
            //    emission.rateOverTime = 10f * wheel.GetSlipping();
            //}
        }

        private void HandleSkidding()
        {
            if (skidVFX == null)
                return;

            isLocked = false; // wheel.IsLocked(minRange: 1f, maxRange: 15f);

            if (!wheel.IsGrounded || !isLocked)
            {
                skidVFX.emitting = false;
                return;
            }

            const float V = 0.02f;

            var point = wheel.Hit.point;
            point.y += V;

            skidVFX.transform.position = point;
            skidVFX.emitting = true;
        }
    }
}