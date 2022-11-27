using NaughtyAttributes;
using UnityEngine;

using static Drifter.Extensions.DrifterExtensions;
using static Drifter.Utility.MathUtility;

namespace Drifter.Components
{
    [System.Serializable]
    public class ClutchComponent : BaseComponent
    {
        [field: Header("General Settings")]
        [field: SerializeField] public ClutchType Type { get; set; } = ClutchType.FrictionDisc;
        [field: SerializeField] public float TorqueCapacity { get; set; } = 400f;
        //[SerializeField] float m_Inertia = 1.5f;
        [field: SerializeField, AllowNesting, ShowIf(nameof(Type), ClutchType.TorqueConverter)] public float Stiffness { get; set; } = 50f;
        [field: SerializeField, Range(0f, 0.9f)] public float Damping { get; set; } = 0.7f;

        public float AngularVelocity { get; set; } = 0f;
        public float Torque { get; private set; } = 0f;
        public float LockInput { get; private set; } = 0f;

        [ShowNativeProperty] public bool IsEngaged { get; private set; } = false;
        [ShowNativeProperty] public bool IsDisengaged { get; private set; } = false;

        public float GetRPM() => AngularVelocity.ToRPM();

        public override void OnEnable(BaseVehicle vehicle) { }
        public override void OnDisable(BaseVehicle vehicle) { }

        public void Simulate(float clutchInput, float outputShaftVelocity, EngineComponent engine, float outputRatio)
        {
            AngularVelocity = Mathf.Max(outputShaftVelocity, 0f);
            var slip = (engine.AngularVelocity - AngularVelocity) * Sign(Mathf.Abs(outputRatio));

            //const float MIN_RANGE = 900f;
            //const float MAX_RANGE = 1900f;

            var rangeVelocity = RemapClamped
            (
                engine.GetRPM(),
                inA: engine.IdleRPM,
                inB: engine.IdleRPM * 2f,
                outA: 0f,
                outB: 1f
            );

            LockInput = Type switch
            {
                ClutchType.FrictionDisc => 1f - clutchInput,
                ClutchType.TorqueConverter => Mathf.Min(rangeVelocity + (outputRatio == 0f ? 1f : 0f), b: 1f),
                _ => 1f,
            };

            var currentTorque = Mathf.Clamp(slip * Stiffness * LockInput, -TorqueCapacity, TorqueCapacity);

            //if (currentTorque > m_TorqueCapacity)
            //{
            //    currentTorque = m_TorqueCapacity;
            //    Disengaged = true;
            //}
            //else if (currentTorque < -m_TorqueCapacity)
            //{
            //    currentTorque = -m_TorqueCapacity;
            //    Disengaged = true;
            //}
            //else
            //    Disengaged = false;

            const float THRESHOLD = 0.5f;
            IsEngaged = (1f - LockInput) > THRESHOLD ? true : false;

            Torque = currentTorque + ((Torque - currentTorque) * Damping);
        }


        #region Data Saving

        public override void Load(FileData data)
        {
            data.ReadValue("Clutch", "Type", out ClutchType type);
            data.ReadValue("Clutch", "TorqueCapacity", out float torqueCapacity);

            Type = type;
            TorqueCapacity = torqueCapacity;

            if (Type == ClutchType.TorqueConverter)
            {
                data.ReadValue("Clutch", "Stiffness", out float stiffness);
                Stiffness = stiffness;
            }

            data.ReadValue("Clutch", "Damping", out float damping);
            Damping = damping;
        }

        public override FileData Save()
        {
            var data = new FileData();

            data.WriteValue("Clutch", "Type", Type);
            data.WriteValue("Clutch", "TorqueCapacity", TorqueCapacity);

            if (Type == ClutchType.TorqueConverter)
                data.WriteValue("Clutch", "Stiffness", Stiffness);
            
            data.WriteValue("Clutch", "Damping", Damping);

            return data;
        }

        #endregion
    }
}