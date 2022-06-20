using NaughtyAttributes;
using UnityEngine;

namespace Drifter.Components
{
    [System.Serializable]
    public class AxleComponent : BaseComponent
    {
        public bool IsGrounded => LeftWheel.IsGrounded || RightWheel.IsGrounded;

        [field: Header("References")]
        [field: SerializeField] public WheelBehaviour LeftWheel { get; private set; } = default;
        [field: SerializeField] public WheelBehaviour RightWheel { get; private set; } = default;

        [field: Header("General Settings")]
        [field: SerializeField] public DifferentialType Type { get; set; } = DifferentialType.Open;
        [field: SerializeField, AllowNesting, ShowIf(nameof(Type), DifferentialType.Locked), Min(1f)] public float Inertia { get; set; } = 1.5f;
        [field: SerializeField, Min(1f)] public float FinalDriveRatio { get; set; } = 3.5f;

        public void ApplySteering(float steerAngle)
        {
            LeftWheel.SteerAngle = steerAngle;
            RightWheel.SteerAngle = steerAngle;
        }

        public void ApplySteering((float left, float right) steerAngle)
        {
            //var leftAngle = Mathf.Rad2Deg * (LeftWheel.AligningTorque * Inertia);
            //var rightAngle = Mathf.Rad2Deg * (RightWheel.AligningTorque * Inertia);

            //steerAngle.left += leftAngle;
            //steerAngle.right += rightAngle;

            LeftWheel.SteerAngle = steerAngle.left;
            RightWheel.SteerAngle = steerAngle.right;
        }

        public void ApplyMotor(float torque)
        {
            LeftWheel.ApplyTorque(torque);
            RightWheel.ApplyTorque(torque);
        }

        public void ApplyMotor((float left, float right) torque)
        {
            LeftWheel.ApplyTorque(torque.left);
            RightWheel.ApplyTorque(torque.right);
        }

        public void ApplyBrake(float input)
        {
            LeftWheel.ApplyBrake(input);
            RightWheel.ApplyBrake(input);
        }

        public void ApplyAntiRollBar(float force)
        {
            var input = LeftWheel.Compression - RightWheel.Compression;

            LeftWheel.ApplyForce(force * input * Vector3.up);
            RightWheel.ApplyForce(force * input * Vector3.down);
        }

        public float GetInputShaftVelocity() => (LeftWheel.AngularVelocity + RightWheel.AngularVelocity) * 0.5f * FinalDriveRatio;

        public float GetLinearVelocity() => (LeftWheel.VelocityAtWheel.z + RightWheel.VelocityAtWheel.z) * 0.5f;

        public (float leftTorque, float rightTorque) GetOutputTorque(float deltaTime, float inputTorque)
        {
            var openTorque = inputTorque * FinalDriveRatio * 0.5f;

            var lockTorque = (LeftWheel.AngularVelocity - RightWheel.AngularVelocity) * 0.5f / deltaTime * Inertia; // 100% Lock Torque

            return Type switch
            {
                DifferentialType.Open => (openTorque, openTorque),
                DifferentialType.Locked => (openTorque - lockTorque, openTorque + lockTorque),
                _ => (openTorque, openTorque)
            };
        }

        public override void Init(BaseVehicle vehicle)
        {

        }

        public override void Simulate(float deltaTime, IVehicleData data = null)
        {

        }

        #region Data Saving

        public override void LoadData(FileData data)
        {
            data.ReadValue("Differential", "Type", out DifferentialType type);

            Type = type;

            if (Type == DifferentialType.Locked)
            {
                data.ReadValue("Differential", "Inertia", out float inertia);
                Inertia = inertia;
            }

            data.ReadValue("Differential", "FinalDriveRatio", out float finalDriveRatio);
            FinalDriveRatio = finalDriveRatio;
        }

        public override FileData SaveData()
        {
            var data = new FileData();

            data.WriteValue("Differential", "Type", Type);

            if (Type == DifferentialType.Locked)
                data.WriteValue("Differential", "Inertia", Inertia);

            data.WriteValue("Differential", "FinalDriveRatio", FinalDriveRatio);

            return data;
        }

        public override void Shutdown() => throw new System.NotImplementedException();

        #endregion
    }
}