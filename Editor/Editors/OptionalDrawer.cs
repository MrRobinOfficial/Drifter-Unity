using UnityEditor;
using UnityEngine;

namespace Drifter.Editor.Editors
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalDrawer : PropertyDrawer
    {
        private const float WIDTH = 24;
        private const string VALUE_PROPERTY = "m_Value";
        private const string ENABLED_PROPERTY = "m_Enabled";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative(VALUE_PROPERTY);

            if (valueProperty == null)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUI.GetPropertyHeight(valueProperty);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative(VALUE_PROPERTY);
            var enabledProperty = property.FindPropertyRelative(ENABLED_PROPERTY);

            if (enabledProperty == null ||
                valueProperty == null)
                return;

            position.width -= WIDTH;
            EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
            EditorGUI.PropertyField(position, valueProperty, label, includeChildren: true);
            EditorGUI.EndDisabledGroup();

            position.x += position.width + WIDTH;
            position.width = position.height = EditorGUI.GetPropertyHeight(enabledProperty);
            position.x -= position.width;
            EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
        }
    } 
}