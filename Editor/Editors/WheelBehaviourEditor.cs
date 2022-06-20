using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

using Drifter.Components;
using Drifter.Extensions;

using static Drifter.Extensions.DrifterEditorExtensions;
using static Drifter.Editor.Extensions.DrifterEditorUIExtensions;
using UnityEditor.UIElements;
using NaughtyAttributes.Editor;

namespace Drifter.Editor.Editors
{
    [CustomEditor(typeof(WheelBehaviour), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class WheelBehaviourEditor : NaughtyInspector
    {
        public override VisualElement CreateInspectorGUI()
        {
            var wheel = (WheelBehaviour)target;
            var root = new VisualElement();

            var referenceCategory = CreateCategory("References");
            referenceCategory.Add(serializedObject.CreatePropertyField(nameof(wheel.Suspension)));
            referenceCategory.Add(serializedObject.CreatePropertyField(nameof(wheel.TireFriction)));
            referenceCategory.Add(serializedObject.CreatePropertyField(nameof(wheel.Visual)));
            root.Add(referenceCategory);

            var generalCategory = CreateCategory("General Settings");
            generalCategory.Add(serializedObject.CreatePropertyField(nameof(wheel.RaycastCast)));
            generalCategory.Add(serializedObject.CreatePropertyField(nameof(wheel.Center)));
            generalCategory.Add(serializedObject.CreatePropertyField(nameof(wheel.CollideWith)));

            var massField = CreateMassField();
            massField.label = "Mass";
            massField.value = wheel.Mass;
            massField.RegisterValueChangedCallback(ctx => wheel.Mass = ctx.newValue);
            generalCategory.Add(massField);

            var widthField = CreateLengthField();
            widthField.label = "Width";
            widthField.value = wheel.Width;
            widthField.RegisterValueChangedCallback(ctx => wheel.Width = ctx.newValue);
            generalCategory.Add(widthField);

            var radiusField = CreateLengthField();
            radiusField.label = "Radius";
            radiusField.value = wheel.Radius;
            radiusField.RegisterValueChangedCallback(ctx => wheel.Radius = ctx.newValue);
            generalCategory.Add(radiusField);

            generalCategory.Add(serializedObject.CreatePropertyField(nameof(wheel.RollingResistance)));
            generalCategory.Add(serializedObject.CreatePropertyField(nameof(wheel.MaxBrakeTorque)));

            root.Add(generalCategory);

            return root;
        }
    }
}