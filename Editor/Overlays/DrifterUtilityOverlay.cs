using Drifter.Extensions;
using Drifter.Vehicles;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

using static Drifter.Extensions.DrifterEditorExtensions;

namespace Drifter.Editor.Overlays
{
    [Overlay(typeof(SceneView), displayName: "Drifter [Utility]", id = k_Id)]
    [Icon(path: "Assets/Drifter/Editor/Resources/Textures/Icons/Car.psd")]
    [HelpURL(HELP_URL)]
    public class DrifterUtilityOverlay : Overlay
    {
        private const string k_Id = "drifter-utility-overlay";

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();

            root.Add(DrawDefault());

            root.schedule
                .Execute(() => Refresh())
                .Every(intervalMs: 500);

            return root;

            void Refresh()
            {
                root.Clear();
                root.Add(DrawDefault());
                root.Add(HandleSelection());
            }
        }

        private EnumField unitField;

        private VisualElement DrawDefault()
        {
            var defaultRoot = new VisualElement();

            unitField = new EnumField("Unit", Unit);
            unitField.RegisterValueChangedCallback(ctx => Unit = (UnitType)ctx.newValue);

            defaultRoot.Add(unitField);

            var linkLabel = new Label("Click here for help");
            linkLabel.style.color = new(new Color32(22, 79, 171, 255));
            linkLabel.style.marginTop = new Length(4, LengthUnit.Pixel);
            linkLabel.style.marginLeft = new Length(4, LengthUnit.Pixel);
            linkLabel.style.marginRight = new Length(4, LengthUnit.Pixel);
            linkLabel.style.marginBottom = new Length(4, LengthUnit.Pixel);

            linkLabel.RegisterCallback<ClickEvent>(ctx => Application.OpenURL(HELP_URL));

            defaultRoot.Add(linkLabel);

            return defaultRoot;
        }

        private VisualElement HandleSelection()
        {
            //var obj = Selection.activeGameObject;

            //if (obj == null)
            //    return default;

            //var vehicle = obj.GetComponent<BaseVehicle>();

            //if (vehicle == null)
            //    return default;

            //var baseVehicleRoot = DrawVehicle();

            //if (vehicle is CarVehicle)
            //{
            //    carVehicleRoot = DrawCarVehicle(vehicle as CarVehicle);
            //    baseVehicleRoot.Add(carVehicleRoot);
            //}

            //selectionRoot.Add(baseVehicleRoot);

            //VisualElement DrawVehicle()
            //{
            //    var foldout = new Foldout
            //    {
            //        text = "Base [Vehicle]",
            //        value = false,
            //    };

            //    return foldout;
            //}

            //VisualElement DrawCarVehicle(CarVehicle car)
            //{
            //    var foldout = new Foldout
            //    {
            //        text = "Car [Vehicle]",
            //        value = false,
            //    };

            //    return foldout;
            //}

            return new VisualElement();
        }
    }
}