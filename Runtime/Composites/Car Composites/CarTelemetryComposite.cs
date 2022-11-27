#if false
using System.Collections.Generic;
using System.Collections;

using Drifter.Components;
using Drifter.Extensions;
using Drifter.Vehicles;

using UnityEngine;
using UnityEngine.UIElements;
using UnityInput = UnityEngine.Input;

using static Drifter.Extensions.WheelExtensions;
using static Drifter.Extensions.DrifterExtensions;

namespace Drifter.Composites.CarComposites
{
    [AddComponentMenu("Tools/Drifter/Composites/Car Telemetry [Composite]"), DisallowMultipleComponent]
    [RequireComponent(typeof(CarVehicle))]
    public class CarTelemetryComposite : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] UIDocument m_UIDocument = default;

        [Header("General Settings")]
        [SerializeField] bool m_EnableDrawing = false;

        private CarVehicle car;

        private void OnValidate() => m_UIDocument.enabled = m_EnableDrawing;

        private void Awake()
        {
            car = GetComponent<CarVehicle>();

            var root = m_UIDocument.rootVisualElement;
            CreateTelemetry(root.Q("container"));

            StartCoroutine(RefreshUI());
        }

        private WaitForSecondsRealtime shortWait = new WaitForSecondsRealtime(time: 0.01f);

        private IEnumerator RefreshUI()
        {
            while (true)
            {
                foreach (var item in wheels)
                {
                    var wheel = item.Key;
                    var root = item.Value;

                    root.rpm.text = $"{wheel.GetRPM():N0}";
                    root.steering.text = $"{wheel.SteerAngle:0.0}";
                    root.suspension.text = $"{wheel.Compression:P}";
                    root.load.text = $"{wheel.NormalForce:N0}";

                    root.motorTorque.text = $"{wheel.MotorTorque:N0}";
                    root.brakeTorque.text = $"{wheel.BrakeTorque:N0}";
                    root.tractionTorque.text = $"{wheel.TractionTorque:N0}";

                    root.lateralForce.text = $"{wheel.LateralForce:N0}";
                    root.longitudinalForce.text = $"{wheel.LongitudinalForce:N0}";

                    root.lateralSlip.text = $"{wheel.SlipRatio:0.00}";
                    root.longitudinalSlip.text = $"{wheel.SlipRatio:0.00}";
                    root.combinedSlip.text = $"{wheel.GetCombinedSlip().magnitude:0.00}";
                }

                var velocity = car.transform.InverseTransformDirection(car.Body.velocity);
                velocity.y = 0f;

                nameLabel.text = car.name;
                massLabel.text = $"{car.Body.mass:N0} [kg]";

                var inertia = car.Body.inertiaTensor;

                var inertiaX = $"{inertia.z:N0}";
                var inertiaY = $"{inertia.y:N0}";
                var inertiaZ = $"{inertia.z:N0}";

                inertiaLabel.text = $"({inertiaX}; {inertiaY}; {inertiaZ}) [kg*m^2]";

                lateralVelocity.text = $"{velocity.x:00.00}";
                longitudinalVelocity.text = $"{velocity.z:00.00}";
                combinedVelocity.text = $"{velocity.magnitude:00.00}";

                engineRunning.text = $"{(car.Engine.IsRunning ? "on" : "off")}";
                engineLoad.text = $"{car.Engine.GetLoadNormalized():P}";
                engineTorque.text = $"{car.Engine.GetCurrentTorque():N1} [N/m]";
                enginePower.text = $"{car.Engine.GetCurrentPower():N1} [kW]";

                clutchLock.text = $"{car.Clutch.LockInput:P}";
                clutchTorque.text = $"{car.Clutch.Torque:N1}";

                driveshaftRPM.text = $"{car.Clutch.AngularVelocity.ToRPM():N0} [RPM]";

                speedometer.text = $"{car.Speedometer:00.0} [km/h]";
                tachometer.text = $"{car.Tachometer:N0} [RPM]";
                odometer.text = $"{car.Odometer:N0} [km]";
                gear.text = car.Gearbox.GetCurrentGearText();

                yield return shortWait;
            }
        }

        private struct WheelUI
        {
            public readonly VisualElement root;
            public readonly Label name;
            public readonly Label rpm;
            public readonly Label steering;
            public readonly Label suspension;
            public readonly Label load;
            public readonly Label motorTorque;
            public readonly Label brakeTorque;
            public readonly Label tractionTorque;
            public readonly Label lateralForce;
            public readonly Label longitudinalForce;
            public readonly Label lateralSlip;
            public readonly Label longitudinalSlip;
            public readonly Label combinedSlip;

            public float wheelRPM;
            public float steerAngle;
            public float compression;
            public float normalForce;
            //public float motorTorque;

            public WheelUI(WheelBehaviour wheel)
            {
                wheelRPM = default;
                steerAngle = default;
                compression = default;
                normalForce = default;

                root = CreateRowContainer();

                root.Add(name = CreateLabel(wheel.name));

                root.Add(CreateVerticalBar());

                root.Add(rpm = CreateLabel("0,000"));

                root.Add(CreateVerticalBar());

                root.Add(steering = CreateLabel("0.0"));

                root.Add(CreateVerticalBar());

                root.Add(suspension = CreateLabel("0"));

                root.Add(CreateVerticalBar());

                root.Add(load = CreateLabel("0,000"));

                root.Add(CreateVerticalBar());

                root.Add(motorTorque = CreateLabel("0,000"));
                root.Add(brakeTorque = CreateLabel("0,000"));
                root.Add(tractionTorque = CreateLabel("0,000"));

                root.Add(CreateVerticalBar());

                root.Add(lateralForce = CreateLabel("0,000"));
                root.Add(longitudinalForce = CreateLabel("0,000"));

                root.Add(CreateVerticalBar());

                root.Add(lateralSlip = CreateLabel("0.00"));
                root.Add(longitudinalSlip = CreateLabel("0.00"));
                root.Add(combinedSlip = CreateLabel("0.00"));
            }
        }

        private readonly Dictionary<WheelBehaviour, WheelUI> wheels = new();

        private void CreateTelemetry(VisualElement container)
        {
            longitudinalVelocity = CreateLabel("00 [m/s]");
            lateralVelocity = CreateLabel("00 [m/s]");
            combinedVelocity = CreateLabel("00 [m/s]");

            engineRunning = CreateLabel("off");
            engineLoad = CreateLabel("00.0 [%]");
            engineTorque = CreateLabel("0,000.0 [N/m]");
            enginePower = CreateLabel("0,000.0 [kW]");

            clutchLock = CreateLabel("0.0%");
            clutchTorque = CreateLabel("0,000.0 [N/m]");

            driveshaftRPM = CreateLabel("0,000 [RPM]");

            speedometer = CreateLabel("00 [km/h]");
            tachometer = CreateLabel("0,000 [RPM]");
            odometer = CreateLabel("000,000 [km]");
            gear = CreateLabel("N");

            abs = CreateLabel(string.Empty);
            tcs = CreateLabel(string.Empty);
            esc = CreateLabel(string.Empty);

            nameLabel = CreateLabel(car.name);
            massLabel = CreateLabel("0,000 [kg]");
            inertiaLabel = CreateLabel("(0,000; 0,000; 0,000) [kg*m^2]");

            var infoContainer = CreateRowContainer();
            infoContainer.Add(CreateLabel(text: "Quick Info (name/mass/inertia)"));
            infoContainer.Add(nameLabel);
            infoContainer.Add(massLabel);
            infoContainer.Add(inertiaLabel);
            container.Add(infoContainer);

            var wheelContainer = CreateColumnContainer();
            var wheelInfo = CreateRowContainer();

            wheelInfo.Add(CreateLabel("Spin [RPM]"));
            wheelInfo.Add(CreateVerticalBar());
            wheelInfo.Add(CreateLabel("Steer [°]"));
            wheelInfo.Add(CreateVerticalBar());
            wheelInfo.Add(CreateLabel("Susp [%]"));
            wheelInfo.Add(CreateVerticalBar());
            wheelInfo.Add(CreateLabel("Load [N]"));
            wheelInfo.Add(CreateVerticalBar());
            wheelInfo.Add(CreateLabel("Torque (motor/brake/tract) [N/m]"));
            wheelInfo.Add(CreateVerticalBar());
            wheelInfo.Add(CreateLabel("Force (lat/long) [N]"));
            wheelInfo.Add(CreateVerticalBar());
            wheelInfo.Add(CreateLabel("Slip (lat/long/comb) [m/s]"));
            wheelInfo.Add(CreateVerticalBar());

            wheelContainer.Add(wheelInfo);

            foreach (var wheel in car.WheelArray)
            {
                var ui = new WheelUI(wheel);
                wheelContainer.Add(ui.root);
                wheels.Add(wheel, ui);
            }

            container.Add(wheelContainer);

            var speedContainer = CreateRowContainer();
            speedContainer.Add(CreateLabel("Speed (long/lat/comb"));
            speedContainer.Add(longitudinalVelocity);
            speedContainer.Add(lateralVelocity);
            speedContainer.Add(combinedVelocity);
            speedContainer.Add(CreateLabel("[m/s]"));
            container.Add(speedContainer);

            var engineContainer = CreateRowContainer();
            engineContainer.Add(CreateLabel("Engine (ignition/load/torque/power"));
            engineContainer.Add(engineRunning);
            engineContainer.Add(engineLoad);
            engineContainer.Add(engineTorque);
            engineContainer.Add(enginePower);
            container.Add(engineContainer);

            var clutchContainer = CreateRowContainer();
            clutchContainer.Add(CreateLabel("Clutch (lock/torque"));
            clutchContainer.Add(clutchLock);
            clutchContainer.Add(clutchTorque);
            container.Add(clutchContainer);

            var driveshaftContainer = CreateRowContainer();
            driveshaftContainer.Add(CreateLabel("Driveshaft"));
            driveshaftContainer.Add(driveshaftRPM);
            container.Add(driveshaftContainer);

            var dashboardContainer = CreateRowContainer();
            dashboardContainer.AddToClassList("dashboardContainer");

            dashboardContainer.Add(speedometer);
            dashboardContainer.Add(tachometer);
            dashboardContainer.Add(odometer);
            dashboardContainer.Add(gear);

            dashboardContainer.Add(abs);
            dashboardContainer.Add(tcs);
            dashboardContainer.Add(esc);

            container.Add(dashboardContainer);
        }

        private Label nameLabel;
        private Label massLabel;
        private Label inertiaLabel;

        private Label engineRunning;
        private Label engineLoad;
        private Label engineTorque;
        private Label enginePower;

        private Label clutchLock;
        private Label clutchTorque;

        private Label driveshaftRPM;

        private Label longitudinalVelocity;
        private Label lateralVelocity;
        private Label combinedVelocity;

        private Label speedometer;
        private Label tachometer;
        private Label odometer;
        private Label gear;
        private Label abs;
        private Label tcs;
        private Label esc;

        private static Label CreateVerticalBar() => CreateLabel("|");

        private static Label CreateLabel(string text)
        {
            var element = new Label(text);
            element.AddToClassList(className: "label");
            return element;
        }

        private static VisualElement CreateRowContainer()
        {
            var element = new VisualElement();
            element.AddToClassList(className: "rowContainer");
            return element;
        }

        private static VisualElement CreateColumnContainer()
        {
            var element = new VisualElement();
            element.AddToClassList(className: "columnContainer");
            return element;
        }
    }
} 
#endif