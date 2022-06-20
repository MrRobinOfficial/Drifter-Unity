using Drifter.Components;
using Drifter.Extensions;
using Drifter.Utility;
using Drifter.Vehicles;
using UnityEngine;

using static Drifter.Extensions.GUIExtensions;
using static Drifter.Utility.DrifterMathUtility;

namespace Drifter.Composites.CarComposites
{
    [AddComponentMenu("Tools/Drifter/Composites/Car Telemetry GUI [Composite]")]
    [RequireComponent(typeof(CarVehicle))]
    public class CarTelemetryGUIComposite : BaseCarComposite
    {
        [Header("References")]
        [SerializeField] Font m_Font = default;

        private const float SCREEN_WIDTH = 1280f;
        private const float SCREEN_HEIGHT = 720f;

        private const float WINDOW_WIDTH = 750f;
        private const float WINDOW_HEIGHT = 310f;

        private Rect windowRect = new Rect(x: 4, y: 4, WINDOW_WIDTH, WINDOW_HEIGHT);
        private const float RATIO = 16f / 9f;

        private void OnGUI()
        {
            //var resX = Screen.width / SCREEN_WIDTH;
            //var resY = Screen.height / SCREEN_HEIGHT;

            //var matrix = GUI.matrix;

            //GUI.matrix = Matrix4x4.TRS
            //(
            //    Vector3.zero,
            //    Quaternion.identity,
            //    s: new Vector3(resX, resY, z: 1f)
            //);

            var window = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(left: 8, right: 8, top: 8, bottom: 8),
                border = new RectOffset(left: 8, right: 8, top: 8, bottom: 8),
                fontSize = 18,
                fontStyle = FontStyle.Bold,
            };

            if (window != null)
                window.font = m_Font;

            windowRect = GUILayout.Window(id: 0, windowRect, DrawWindow, GUIContent.none, window);

            //GUI.matrix = matrix;
        }

        private void DrawWindow(int windowID)
        {
            var engine = carVehicle.Engine;
            var clutch = carVehicle.Clutch;
            var gearbox = carVehicle.Gearbox;
            var frontAxle = carVehicle.FrontAxle;
            var rearAxle = carVehicle.RearAxle;

            var label = new GUIStyle(GUI.skin.label)
            {
                wordWrap = true,
                richText = true,
                fixedWidth = 90f,
            };

            if (label != null)
                label.font = m_Font;

            var titleLabel = new GUIStyle(label)
            {
                fixedWidth = 300f,
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
            };

            var rowTitleLabel = new GUIStyle(label)
            {
                fontStyle = FontStyle.Bold,
                fixedWidth = 200,
            };

            var rowLabel = new GUIStyle(label)
            {
                margin = new RectOffset(left: 0, right: 10, top: 0, bottom: 0),
            };

            var rowLongLabel = new GUIStyle(rowLabel)
            {
                fixedWidth = 110f,
            };

            var quickLabel = new GUIStyle(label)
            {
                fixedWidth = 200f,
                fontSize = 22,
                fontStyle = FontStyle.Bold,
            };

            var wheelTitleLabel = new GUIStyle(rowTitleLabel)
            {
                fixedWidth = 70f,
            };

            var wheelLongTitleLabel = new GUIStyle(wheelTitleLabel)
            {
                fixedWidth = 120f,
            };

            var wheelRowLabel = new GUIStyle(rowLabel)
            {
                fixedWidth = 60f,
            };

            var wheelLongRowLabel = new GUIStyle(wheelRowLabel)
            {
                fixedWidth = 70f,
            };

            var column = new GUIStyle()
            {
                margin = new RectOffset(left: 4, right: 4, top: 4, bottom: 14),
            };

            var row = new GUIStyle();

            var container = new GUIStyle()
            {
                padding = new RectOffset(left: 16, right: 16, top: 16, bottom: 16),
                margin = new RectOffset(left: 4, right: 4, top: 4, bottom: 4),
            };

            GUILayout.Label(text: "Telemetry", titleLabel);

            /// Container ///
            GUILayout.BeginVertical(container);

            /// Vehicle Info ///
            GUILayout.BeginVertical(column);
            GUILayout.BeginHorizontal(row);
            DrawLabel(text: "Vehicle (name/curb/gross)", rowTitleLabel);
            DrawLabel(text: $"{carVehicle.name}", rowLabel);
            DrawLabel(text: $"{carVehicle.Mass:N0} [kg]", rowLabel);
            DrawLabel(text: $"{carVehicle.Mass:N0} [kg]", rowLabel);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            /// Wheels Info ///
            GUILayout.BeginVertical(column);
            GUILayout.BeginHorizontal(row);

            /// Wheel's Name ///
            GUILayout.BeginVertical();
            DrawLabel(text: "Name\n[string]", wheelTitleLabel);
            for (int i = 0; i < carVehicle.WheelArray.Length; i++)
                DrawLabel(carVehicle.WheelArray[i].name, wheelTitleLabel);
            GUILayout.EndVertical();

            /// Wheel's Contact ///
            GUILayout.BeginVertical();
            DrawLabel(text: "Contact\n[boolean]", wheelTitleLabel);
            for (int i = 0; i < carVehicle.WheelArray.Length; i++)
                DrawLabel(text: $"{carVehicle.WheelArray[i].IsGrounded}", wheelRowLabel);
            GUILayout.EndVertical();

            /// Wheel's RPM ///
            GUILayout.BeginVertical();
            DrawLabel(text: "Spin\n[RPM]", wheelTitleLabel);
            for (int i = 0; i < carVehicle.WheelArray.Length; i++)
                DrawLabel(text: $"{carVehicle.WheelArray[i].GetRPM():N0}", wheelRowLabel);
            GUILayout.EndVertical();

            /// Wheel's Suspension ///
            GUILayout.BeginVertical();
            DrawLabel(text: "Susp\n[%]", wheelTitleLabel);
            for (int i = 0; i < carVehicle.WheelArray.Length; i++)
                DrawLabel(text: $"{carVehicle.WheelArray[i].Compression:P}", wheelRowLabel);
            GUILayout.EndVertical();

            /// Wheel's Load ///
            GUILayout.BeginVertical();
            DrawLabel(text: "Load\n[N]", wheelTitleLabel);
            for (int i = 0; i < carVehicle.WheelArray.Length; i++)
                DrawLabel(text: $"{carVehicle.WheelArray[i].NormalForce:N0}", wheelLongRowLabel);
            GUILayout.EndVertical();

            /// Wheel's Torque ///
            GUILayout.BeginVertical();
            DrawLabel(text: "Torque (motor/brake/tract)\n[Nm]", wheelLongTitleLabel);

            for (int i = 0; i < carVehicle.WheelArray.Length; i++)
            {
                GUILayout.BeginHorizontal(row);
                DrawLabel(text: $"Tm: {carVehicle.WheelArray[i].MotorTorque:N0}", wheelLongRowLabel);
                DrawLabel(text: $"Tb: {carVehicle.WheelArray[i].BrakeTorque:N0}", wheelLongRowLabel);
                DrawLabel(text: $"Tt: {carVehicle.WheelArray[i].TractionTorque:N0}", wheelLongRowLabel);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            /// Wheel's Force ///
            GUILayout.BeginVertical();
            DrawLabel(text: "Force (long/lat)\n[N]", wheelLongTitleLabel);

            for (int i = 0; i < carVehicle.WheelArray.Length; i++)
            {
                GUILayout.BeginHorizontal(row);
                DrawLabel(text: $"Fy: {carVehicle.WheelArray[i].LongitudinalForce:N0}", wheelLongRowLabel);
                DrawLabel(text: $"Fx: {carVehicle.WheelArray[i].LateralForce:N0}", wheelLongRowLabel);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            /// Wheel's Slip ///
            GUILayout.BeginVertical();
            DrawLabel(text: "Slip (long/lat/comb)\n[m/s]", wheelLongTitleLabel);

            for (int i = 0; i < carVehicle.WheelArray.Length; i++)
            {
                GUILayout.BeginHorizontal(row);
                DrawLabel(text: $"Sy: {carVehicle.WheelArray[i].SlipRatio:0.00}", wheelLongRowLabel);
                DrawLabel(text: $"Sx: {carVehicle.WheelArray[i].SlipAngle:0.00}", wheelLongRowLabel);
                DrawLabel(text: $"Sc: {carVehicle.WheelArray[i].GetCombinedSlip().magnitude:0.00}", wheelLongRowLabel);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            /// Powertrain ///
            GUILayout.BeginVertical(column);

            /// ENGINE ///
            GUILayout.BeginHorizontal(row);
            DrawLabel(text: "Engine (load/torque/power)", rowTitleLabel);
            DrawLabel(text: $"{engine.GetLoadNormalized():P}", rowLabel);
            DrawLabel(text: $"{engine.GetCurrentTorque():N0} [N/m]", rowLabel);
            DrawLabel(text: $"{engine.GetCurrentPower():N1} [kW]", rowLongLabel);
            GUILayout.EndHorizontal();

            /// CLUTCH ///
            GUILayout.BeginHorizontal(row);
            DrawLabel(text: "Clutch (lock/torque)", rowTitleLabel);
            DrawLabel(text: $"{clutch.LockInput:P}", rowLabel);
            DrawLabel(text: $"{clutch.Torque:N0} [N/m]", rowLabel);
            GUILayout.EndHorizontal();

            /// GEARBOX ///
            GUILayout.BeginHorizontal(row);
            DrawLabel(text: "Gearbox (rpm)", rowTitleLabel);
            DrawLabel(text: $"{clutch.GetRPM():N0} [RPM]", rowLongLabel);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            /// GAUGES ///
            GUILayout.BeginVertical(column);
            GUILayout.BeginHorizontal(row);
            DrawLabel(text: "Gauges (Speed/Tacho/Odo)", rowTitleLabel);
            DrawLabel(text: $"{carVehicle.Speedometer.Value:N0} [km/h]", rowLabel);
            DrawLabel(text: $"{carVehicle.Tachometer.Value:N0} [RPM]", rowLabel);
            DrawLabel(text: $"{carVehicle.Odometer.Value:N} [km]", rowLabel);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            var input = new GUIStyle(label)
            {
                fixedWidth = 30,
            };

            /// INPUTS ///
            GUILayout.BeginVertical(column);
            GUILayout.BeginHorizontal(row);
            DrawLabel(text: "Input (str/thr/brk/clth/hndbrk)", rowTitleLabel);
            DrawLabel(text: $"{carVehicle.SteerInput:0.0}", input);
            DrawLabel(text: $"{carVehicle.ThrottleInput:0.0}", input);
            DrawLabel(text: $"{carVehicle.BrakeInput:0.0}", input);
            DrawLabel(text: $"{carVehicle.ClutchInput:0.0}", input);
            DrawLabel(text: $"{carVehicle.HandbrakeInput:0.0}", input);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            /// Speed Info ///
            GUILayout.BeginVertical(column);
            GUILayout.BeginHorizontal(row);
            DrawLabel(text: "Speed (long/lat/abs)", rowTitleLabel);
            DrawLabel(text: $"{carVehicle.ForwardVelocity:00.00} [m/s]", rowLabel);
            DrawLabel(text: $"{carVehicle.RightVelocity:00.00} [m/s]", rowLabel);
            DrawLabel(text: $"{carVehicle.LinearVelocity:00.00} [m/s]", rowLabel);
            DrawLabel(text: $"{carVehicle.GetSpeedInKph():N0} [km/h]", rowLabel);
            DrawLabel(text: $"{Mathf.Round(ConvertKphToMph(carVehicle.GetSpeedInKph())):N0} [mph]", label);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            /// Quick Info ///
            GUILayout.BeginVertical(column);
            GUILayout.BeginHorizontal(row);
            DrawLabel(text: $"{carVehicle.Engine.GetRPM():N0} [RPM]", quickLabel);
            DrawLabel(text: carVehicle.Gearbox.GetCurrentGearText(), quickLabel);
            DrawLabel(text: $"{carVehicle.GetSpeedInKph():N0} [km/h]", quickLabel);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }
    }
}