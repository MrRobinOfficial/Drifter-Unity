using Cinemachine;
using UnityEngine;

namespace Drifter.Samples.Cinemachine
{
    [AddComponentMenu(""), DisallowMultipleComponent]
    public class CinemachinePhysicsCameraExtension : CinemachineExtension
    {
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {

        }
    }
}