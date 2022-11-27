using Drifter.Handlers;
using Drifter.Vehicles;
using UnityEngine;
using UnityEngine.Audio;

namespace Drifter.Composites.CarComposites
{
    [AddComponentMenu("Tools/Drifter/Composites/Car Audio [Composite]"), DisallowMultipleComponent]
    [RequireComponent(typeof(CarVehicle))]
    public class CarAudioComposite : MonoBehaviour
    {
        [System.Serializable]
        public class EngineSetup
        {
            public AudioSource source;
            public float minRPM;
            public float peakRPM;
            public float maxRPM;
            public float pitchReferenceRPM;

            public float SetPitchAndGetVolumeForRPM(float rpm)
            {
                source.pitch = rpm / pitchReferenceRPM;

                if (rpm < minRPM || rpm > maxRPM)
                    return 0f;

                return rpm < peakRPM ? Mathf.InverseLerp(minRPM, peakRPM, rpm) : Mathf.InverseLerp(maxRPM, peakRPM, rpm);
            }

            public void SetVolume(float volume) => source.mute = (source.volume = volume) == 0f;
        }

        [Header("References")]
        [SerializeField] AudioMixerGroup m_WheelMixerGroup = default;

        [Header("General Settings")]
        [SerializeField, Range(0f, 1f)] float m_MasterVolume = 1f;

        [Header("Setups")]
        [SerializeField] EngineSetup[] m_EngineSetups = default;

        [Header("Wheel SFX")]
        [SerializeField] AudioClip m_SkidClip = default;

        static float[] workingVolumes = new float[3]; // or maximum number of engine setups you need

        private CarVehicle carVehicle = default;

        private void Awake()
        {
            carVehicle = GetComponent<CarVehicle>();

            //foreach (var wheel in carVehicle.WheelArray)
            //{
            //    var handler = wheel.gameObject.AddComponent<WheelSFXHandler>();
            //    handler.Init(m_SkidClip, m_WheelMixerGroup);
            //}
        }

        private void Update()
        {
            var engine = carVehicle.Engine;

            var totalVolume = 0f;

            for (int i = 0; i < m_EngineSetups.Length; ++i)
                totalVolume += workingVolumes[i] = m_EngineSetups[i].SetPitchAndGetVolumeForRPM(engine.GetRPM());

            if (totalVolume > float.Epsilon)
            {
                for (int i = 0; i < m_EngineSetups.Length; ++i)
                    m_EngineSetups[i].SetVolume(m_MasterVolume * workingVolumes[i] / totalVolume);
            }
        }
    }
}