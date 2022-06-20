using UnityEngine;

namespace Drifter
{
    public enum DriveType : byte
    {
        AWD,
        FWD,
        RWD,
        [InspectorName("4WD")] FourWD,
    }

    public enum TransferCaseType : byte
    {
        Low,
        High,
    }

    public enum SteeringType : byte
    {
        Classic,
        Ackermann,
    }

    public enum CameraType : byte
    {
        Orbit,
        POV,
        Hood,
    }

    public enum DrivingMode : byte
    {
        Normal,
        ECO,
        Sport,
        Profile1,
        Profile2,
        Profile3,
    }

    public enum LightMode : byte
    {
        Off,
        LowBeam,
        FullBeam,
    }

    public enum EngineType : byte
    {
        InternalCombustion,
        Electric,
        IndirectDiesel,
        DirectDiesel,
    }

    public enum TransmissionType : byte
    {
        Manual,
        Automatic,
        AutomaticSequential,
        CVT,
    }

    public enum ClutchType : byte
    {
        TorqueConverter,
        FrictionDisc,
    }

    public enum DifferentialType : byte
    {
        Open,
        Locked,
        ClutchPack,
        Viscous,
        TorqueBias,
    }

    public enum SurfaceType : byte
    {
        Custom,
        DryAsphalt,
        WetAsphalt,
        DryConcrete,
        WetConcrete,
        DryMudd,
        WetMudd,
        Grass,
        Sand,
        Gravel,
        Snow,
        Ice,
    }

    public enum AxisDirection : byte
    {
        WorldUp,
        WorldRight,
        WorldForward,
        WorldBack,
        WorldLeft,
        WorldDown,
        LocalUp,
        LocalDown,
        LocalLeft,
        LocalRight,
        LocalForward,
        LocalBack,
    }
}

namespace Drifter.Extensions
{
    public static partial class DrifterExtensions
    {
        public static readonly string DEFAULT_FILE_PATH = "Vehicles/";

        public static (float min, float max) GetFrictionSurface(bool isStatic, SurfaceType type, (float min, float max) customStatic = default, (float min, float max) customKinetic = default) => type switch
        {
            SurfaceType.DryAsphalt when isStatic => (0.95f, 1f),
            SurfaceType.DryAsphalt when !isStatic => (0.8f, 0.9f),
            SurfaceType.WetAsphalt when isStatic => (0.5f, 0.75f),
            SurfaceType.WetAsphalt when !isStatic => (0.25f, 0.35f),
            SurfaceType.DryConcrete when isStatic => (0.8f, 0.85f),
            SurfaceType.DryConcrete when !isStatic => (0.6f, 0.65f),
            SurfaceType.WetConcrete when isStatic => (0.7f, 0.75f),
            SurfaceType.WetConcrete when !isStatic => (0.45f, 0.5f),
            SurfaceType.DryMudd when isStatic => (0.95f, 1f),
            SurfaceType.DryMudd when !isStatic => (0.8f, 0.9f),
            SurfaceType.WetMudd when isStatic => (0.95f, 1f),
            SurfaceType.WetMudd when !isStatic => (0.8f, 0.9f),
            SurfaceType.Grass when isStatic => (0.95f, 1f),
            SurfaceType.Grass when !isStatic => (0.8f, 0.9f),
            SurfaceType.Gravel when isStatic => (0.95f, 1f),
            SurfaceType.Gravel when !isStatic => (0.8f, 0.9f),
            SurfaceType.Sand when isStatic => (0.35f, 0.4f),
            SurfaceType.Sand when !isStatic => (0.2f, 0.3f),
            SurfaceType.Snow when isStatic => (0.2f, 0.25f),
            SurfaceType.Snow when !isStatic => (0.1f, 0.15f),
            SurfaceType.Ice when isStatic => (0.1f, 0.15f),
            SurfaceType.Ice when !isStatic => (0.03f, 0.1f),
            SurfaceType.Custom when isStatic => customStatic,
            SurfaceType.Custom when !isStatic => customKinetic,
            _ => (0f, 0f),
        };

        public static Vector3 GetAxisDirection(this Transform transform, AxisDirection direction) => direction switch
        {
            AxisDirection.WorldUp => Vector3.up,
            AxisDirection.WorldRight => Vector3.right,
            AxisDirection.WorldForward => Vector3.forward,
            AxisDirection.WorldLeft => Vector3.left,
            AxisDirection.WorldBack => Vector3.back,
            AxisDirection.WorldDown => Vector3.down,
            AxisDirection.LocalUp => transform.up,
            AxisDirection.LocalDown => -transform.up,
            AxisDirection.LocalRight => transform.right,
            AxisDirection.LocalLeft => -transform.right,
            AxisDirection.LocalForward => transform.forward,
            AxisDirection.LocalBack => -transform.forward,
            _ => Vector3.zero,
        };

        //public static float GetFrictionSurface(bool isStatic, SurfaceType surface, float customStatic = 0f, float customKinetic = 0f) => surface switch
        //{
        //    SurfaceType.DryAsphalt when isStatic => 0.95f, // 0.8 -> 0.9
        //    SurfaceType.DryAsphalt when !isStatic => 0.75f, // 0.8 -> 0.9
        //    SurfaceType.WetAsphalt when isStatic => 0.75f, // 0.8 -> 0.9
        //    SurfaceType.WetAsphalt when !isStatic => 0.6f, // 0.8 -> 0.9
        //    SurfaceType.WetConcrete when isStatic => 0.8f,
        //    SurfaceType.WetConcrete when !isStatic => 0.4f,
        //    SurfaceType.DryOffRoad when isStatic => 0.68f,
        //    SurfaceType.DryOffRoad when !isStatic => 0.38f,
        //    SurfaceType.Gravel when isStatic => 0.6f,
        //    SurfaceType.Gravel when !isStatic => 0.4f,
        //    SurfaceType.DryConcrete when isStatic => 0.75f, // 0.5 -> 0.6
        //    SurfaceType.DryConcrete when !isStatic => 0.65f, // 0.5 -> 0.6
        //    SurfaceType.WetConcrete when isStatic => 0.65f, // 0.5 -> 0.6
        //    SurfaceType.WetConcrete when !isStatic => 0.45f, // 0.5 -> 0.6
        //    SurfaceType.WetOffRoad when isStatic => 0.55f,
        //    SurfaceType.WetOffRoad when !isStatic => 0.2f,
        //    SurfaceType.Snow when isStatic => 0.2f,
        //    SurfaceType.Snow when !isStatic => 0.007f,
        //    SurfaceType.Ice when isStatic => 0.1f,
        //    SurfaceType.Ice when !isStatic => 0.03f,
        //    SurfaceType.Grass when isStatic => 0.03f,
        //    SurfaceType.Grass when !isStatic => 0.03f,
        //    SurfaceType.Sand when isStatic => 0.3f,
        //    SurfaceType.Sand when !isStatic => 0.1f,
        //    SurfaceType.WetMudd when isStatic => 0.03f,
        //    SurfaceType.WetMudd when !isStatic => 0.03f,
        //    SurfaceType.DryMudd when isStatic => 0.03f,
        //    SurfaceType.DryMudd when !isStatic => 0.03f,
        //    SurfaceType.Custom when isStatic => customStatic,
        //    SurfaceType.Custom when !isStatic => customKinetic,
        //    _ => 0f,
        //};

        public static float ToRads(this float rpm) => rpm * 0.10472f;
        public static float ToRPM(this float rads) => rads / 0.10472f;

        public static float GetScale(this Transform transform) => transform.lossyScale.magnitude / 1.73205078f;

        public static Vector3 GetSimpleDrag(float dragCoefficient, Vector3 velocity) => -dragCoefficient * velocity * Mathf.Abs(velocity.magnitude);

        public static Vector3 GetAdvancedDrag(float dragCoefficient, Vector3 area, Vector3 velocity, float airDensity = 1.29f) => 0.5f * airDensity * dragCoefficient * area * Mathf.Pow(velocity.magnitude, 2);

        public static Vector3 GetRollingResistance(float rollingCoefficient, Vector3 velocity) => -rollingCoefficient * velocity;

        public static Vector3 ApplyForce(float mass, Vector3 gravity, float slopeAngle = 0f) => mass * Mathf.Sin(slopeAngle) * gravity;

        public static Vector3 Rotated(this Vector3 vector, Quaternion rotation, Vector3 pivot = default(Vector3)) => rotation * (vector - pivot) + pivot;

        public static Vector3 Rotated(this Vector3 vector, Vector3 rotation, Vector3 pivot = default(Vector3)) => Rotated(vector, Quaternion.Euler(rotation), pivot);

        public static Vector3 Rotated(this Vector3 vector, float x, float y, float z, Vector3 pivot = default(Vector3)) => Rotated(vector, Quaternion.Euler(x, y, z), pivot);

#nullable enable
        public static T? TryGetValue<T>(this Optional<T> obj) => obj.IsNotNull ? obj.Value : default;

        public static string AsString(this System.TimeSpan span) => span.ToString(@"hh\:mm\:ss\.ffff");
    }
}