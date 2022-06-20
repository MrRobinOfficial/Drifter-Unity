using UnityEngine;

namespace Drifter.Data
{
    [CreateAssetMenu(menuName = "Tools/Drifter/Presets/Suspension [Preset]", fileName = "New Suspension Preset", order = 20)]
	public class SuspensionPreset : ScriptableObject
	{
        [SerializeField,  Range(6f, 26f)] float m_RideHeight = 10f;

        public float GetRideHeight() => m_RideHeight / 100f; // [cm] -> [m]

        [Header("Spring")]
        [SerializeField,  Tooltip("Spring stiffness rate in [N/mm]"), Range(0, 150)] public uint m_SpringStiffness = 45;
        [SerializeField] public AnimationCurve m_SpringCurve = new(DefaultSpringCurve.keys);

        [Header("Damper")]
        [Tooltip("Damping rate when the suspension is compressing in [N/ms^-1]"), SerializeField] public uint m_DamperBumpStiffness = 3500;
        [Tooltip("Damping rate when the suspension is extending in [N/ms^-1]"), SerializeField] public uint m_DamperReboundStiffness = 4000;
        [SerializeField] public AnimationCurve m_DamperBumpCurve = new(DefaultDamperBumpCurve.keys);
        [SerializeField] public AnimationCurve m_DamperReboundCurve = new(DefaultDamperReboundCurve.keys);

        public static readonly AnimationCurve DefaultSpringCurve = AnimationCurve.EaseInOut(timeStart: 0f, valueStart: 0f, timeEnd: 1f, valueEnd: 1f);

        public static readonly AnimationCurve DefaultDamperBumpCurve = AnimationCurve.EaseInOut(timeStart: 0f, valueStart: 0f, timeEnd: 1f, valueEnd: 1f);

        public static readonly AnimationCurve DefaultDamperReboundCurve = AnimationCurve.EaseInOut(timeStart: 0f, valueStart: 0f, timeEnd: 1f, valueEnd: 1f);

        public bool IsFullCompressed { get; private set; } = false;
        public bool IsFullUncompressed { get; private set; } = false;
    }
}