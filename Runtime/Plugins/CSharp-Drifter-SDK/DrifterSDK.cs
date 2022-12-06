using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

public class DrifterSDK
{
    /// @brief Error Flags
    public enum EErrorFlags
    {
        // Area

        Powertrain = 1 << 0,
        Chassis = 1 << 1,
        Body = 1 << 2,
        Network = 1 << 3,

        // Codes

        StandardCode = 1 << 4,
        ManufacturerSpecificCode = 1 << 5,

        // Subsystem

        IgnitionSystem = 1 << 6,
        EngineIdleControl = 1 << 7,
        TransmissionControlSystem = 1 << 8,
    };

    /// @brief Drive Type
	public enum EDriveType : byte
    {
        /// @brief All-wheel drive
        AWD,
		/// @brief Front wheel drive
		FWD, 
		/// @brief Rear wheel drive
		RWD,
	};

    /// @brief Steering Type
    public enum ESteeringType : byte
    {
        Classic,
		Ackermann,
	};

    /// @brief Driving Mode
    public enum EDrivingMode : byte
    {
        Normal,
		ECO,
		Sport,
		/// @brief Custom preset 1
		Profile1,
		/// @brief Custom preset 2
		Profile2,
		/// @brief Custom preset 3
		Profile3,
	};

    /// @brief Light Mode
    public enum ELightMode : byte
    {
        Off,
		LowBeam,
		FullBeam,
	};

    public enum ELightType : byte
    {
        LeftSignal,
		RightSignal,
		HazardSignal,
		Headlights,
		FogLights,
		Lightbar
    };

    /// @brief Engine Type
    public enum EEngineType : byte
    {
        InternalCombustion,
		Electric,
		IndirectDiesel,
		DirectDiesel,
	};

    /// @brief Transmission Type
    public enum ETransmissionType : byte
    {
        Manual,
		Automatic,
		AutomaticSequential,
		CVT,
	};

    /// @brief Clutch Type
    public enum EClutchType : byte
    {
        TorqueConverter,
		FrictionDisc,
	};

    /// @brief Differential Type
    public enum EDifferentialType : byte
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
	};

    /// @brief Surface Type
    public enum ESurfaceType : byte
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
	};

	public struct WheelFriction_Data
	{

	}

    /// @brief Get gear as text formatted
    /// @param GearIndex Gear index number (negative = R, zero = N, positive = 1,2,3..)
    /// @return A string with gear as text formatted
    [DllImport("Drifter", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    public static extern string GetGearText(int gearIndex);
}
