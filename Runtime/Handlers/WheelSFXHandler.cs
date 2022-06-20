using Drifter.Components;
using UnityEngine;
using UnityEngine.Audio;

namespace Drifter.Handlers
{
    [RequireComponent(typeof(WheelBehaviour))]
    public class WheelSFXHandler : MonoBehaviour
    {
        private WheelBehaviour wheel;
        private AudioSource skidSource;

        private void Awake()
        {
            hideFlags = HideFlags.NotEditable;
            wheel = GetComponent<WheelBehaviour>();

            skidSource = gameObject.AddComponent<AudioSource>();

            skidSource.playOnAwake = false;
            skidSource.loop = true;
            skidSource.spatialBlend = 1f;
            skidSource.priority = 0;
            skidSource.dopplerLevel = 0f;
            skidSource.volume = 1f;
            skidSource.minDistance = 3f;
            skidSource.maxDistance = 100000f;
        }

        public void Init(AudioClip skidClip, AudioMixerGroup mixerGroup = null)
        {
            if (mixerGroup != null)
                skidSource.outputAudioMixerGroup = mixerGroup;

            skidSource.clip = skidClip;
        }

        private void Update() => HandleSkidding();

        private void HandleSkidding()
        {
            if (skidSource.clip == null)
                return;

            const float SPEED_THRESHOLD = 1f;

            if (!wheel.IsGrounded || wheel.VelocityAtWheel.magnitude <= SPEED_THRESHOLD)
            {
                skidSource.Stop();
                return;
            }

            var isLocked = false; // wheel.IsLocked(minRange: 0f, maxRange: 30f);

            if (!skidSource.isPlaying && isLocked)
                skidSource.Play();

            if (skidSource.isPlaying && !isLocked)
                skidSource.Stop();

            //skidSource.volume *= wheel.GetSlipping(minRange: 0f, maxRange: 30f);
        }
    }
}