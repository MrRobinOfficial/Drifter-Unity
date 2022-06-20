using Drifter.Data;
using UnityEngine;

namespace Drifter.Handlers
{
    public sealed class CameraHandler : BaseHandler
    {
        private Camera cam;
        private BaseVehicle vehicle;
        private CameraPreset preset;

        public void Init(BaseVehicle vehicle, CameraPreset preset, Camera cam)
        {
            this.vehicle = vehicle;
            this.preset = preset;
            this.cam = cam;
        }

        private void Update()
        {

        }
    } 
}