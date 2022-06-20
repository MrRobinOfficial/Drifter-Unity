using Drifter.Data;
using Cinemachine;
using UnityEngine;

namespace Drifter.Handlers
{
    public sealed class CinemachineCameraHandler : BaseHandler
    {
        private CinemachineVirtualCamera cam;
        private BaseVehicle vehicle;
        private CameraPreset preset;

        private CinemachineBasicMultiChannelPerlin perlin;

        public void Init(BaseVehicle vehicle, CameraPreset preset, CinemachineVirtualCamera cam)
        {
            this.vehicle = vehicle;
            this.preset = preset;
            this.cam = cam;

            perlin = this.cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public void Init(BaseVehicle vehicle, CameraPreset preset, CinemachineStateDrivenCamera cam)
        {
            this.vehicle = vehicle;
            this.preset = preset;
            this.cam = null;

            perlin = this.cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;

            preset.CalcFOV(ref cam.m_Lens.FieldOfView, deltaTime, vehicle);

            if (perlin != null)
            {
                var shake = preset.CalcShake(deltaTime, vehicle);

                perlin.m_AmplitudeGain = shake.amplitude;
                perlin.m_AmplitudeGain = shake.frequency;
            }
        }
    }
}