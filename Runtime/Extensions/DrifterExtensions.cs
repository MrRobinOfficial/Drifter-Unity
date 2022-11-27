using UnityEngine;

namespace Drifter
{
    [System.Serializable]
    public struct RangeFloat
    {
        [SerializeField] float m_MinValue;
        [SerializeField] float m_MaxValue;

        public RangeFloat(float minLimit, float maxLimit) : this()
        {
            MinValue = minLimit;
            MaxValue = maxLimit;
            MinLimit = minLimit;
            MaxLimit = maxLimit;
        }

        public float GetRandomValue() => Random.Range(MinValue, MaxValue);

        public float MinValue
        {
            get => m_MinValue;
            set => m_MinValue = Mathf.Max(value, MinLimit);
        }

        public float MaxValue
        {
            get => m_MaxValue;
            set => m_MaxValue = Mathf.Min(value, MaxLimit);
        }

        [field: SerializeField] public float MinLimit { get; private set; }
        [field: SerializeField] public float MaxLimit { get; private set; }

        public override string ToString() => $"{MinLimit}, {MaxLimit}";
    }

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
        None,
        Locked,
        Open,
        Viscous,
        LSD,
        //Open,
        //Locked,
        //ClutchPack,
        //Viscous,
        //TorqueBias,
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
#nullable enable

        public static T? TryGetValue<T>(this Optional<T> obj) => 
            obj.Enabled && obj.Value != null ? obj.Value : default;
#nullable disable

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

        public static string GenerateLicensePlate()
        {
            var rnd = new System.Random();

            var letter1 = (char)rnd.Next('a', 'z');
            var letter2 = (char)rnd.Next('a', 'z');
            var letter3 = (char)rnd.Next('a', 'z');

            var number1 = (int)(Random.value * 10);
            var number2 = (int)(Random.value * 10);
            var number3 = (int)(Random.value * 10);

            var builder = new System.Text.StringBuilder(capacity: 6);

            builder.Append(char.ToUpper(letter1));
            builder.Append(char.ToUpper(letter2));
            builder.Append(char.ToUpper(letter3));
            builder.Append('-');
            builder.Append(number1);
            builder.Append(number2);
            builder.Append(number3);

            return builder.ToString();
        }

        public static string GetAutoPropertyName(string propName) => 
            string.Format("<{0}>k__BackingField", propName);

        public static float GetScale(this Transform transform) => 
            transform.lossyScale.magnitude / 1.73205078f;

        public static string AsString(this System.TimeSpan span) => 
            span.ToString(@"hh\:mm\:ss\.ffff");
    }
}