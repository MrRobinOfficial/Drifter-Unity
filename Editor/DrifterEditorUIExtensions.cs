using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using static Drifter.Extensions.DrifterEditorExtensions;

namespace Drifter.Editor.Extensions
{
    public static class DrifterEditorUIExtensions
    {
        private const string MASS_FIELD = "UI/Templates/massField";
        private const string LENGTH_FIELD = "UI/Templates/lengthField";
        private const string VOLUME_FIELD = "UI/Templates/volumeField";

        public static FloatField CreateMassField()
        {
            var root = Resources.Load<VisualTreeAsset>(MASS_FIELD).CloneTree();
            return root.Q<FloatField>();
        }

        public static FloatField CreateLengthField()
        {
            var root = Resources.Load<VisualTreeAsset>(LENGTH_FIELD).CloneTree();
            return root.Q<FloatField>();
        }

        public static FloatField CreateVolumeField()
        {
            var root = Resources.Load<VisualTreeAsset>(VOLUME_FIELD).CloneTree();
            return root.Q<FloatField>();
        }

        public static PropertyField CreatePropertyField(this SerializedObject serializedObject, string propertyName) => new PropertyField(serializedObject.FindPropertyByAutoPropertyName(propertyName));

        public static Label CreateHeader(string text)
        {
            var label = new Label(text);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            return label;
        }

        public static VisualElement CreateCategory(string label)
        {
            var root = new VisualElement();
            root.Add(CreateHeader(label));
            root.style.marginTop = new StyleLength(new Length(10f, LengthUnit.Pixel));
            return root;
        }
    }
}