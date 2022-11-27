using Drifter.Vehicles;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Drifter.Misc
{
    [AddComponentMenu("Tools/Drifter/Misc/Driver Rig [IK]"), DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class IKCarDriverRig : MonoBehaviour
    {
        [field: SerializeField] public CarVehicle Car { get; private set; } = default;

        private static readonly int DRIVING_HASH = Animator.StringToHash("IsDriving");
        private static readonly int STEER_HASH = Animator.StringToHash("SteerInput");
        private static readonly int THROTTLE_HASH = Animator.StringToHash("ThrottleInput");
        private static readonly int BRAKE_HASH = Animator.StringToHash("BrakeInput");
        private static readonly int ENTER_CAR_HASH = Animator.StringToHash("Entering Car");
        private static readonly int EXIT_CAR_HASH = Animator.StringToHash("Exiting Car");

        private Animator animator;

        private void Awake() => animator = GetComponent<Animator>();

        private void Update()
        {
            if (Car == null)
                return;

            animator.SetFloat(STEER_HASH, (0f - Car.GetSteerInput()) * 0.5f + 0.5f);
        }
    } 
}
