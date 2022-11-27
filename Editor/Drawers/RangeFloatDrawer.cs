using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

namespace Drifter.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(RangeFloat))]
    internal sealed class RangeFloatDrawer : PropertyDrawer
    {
        private const string MIN_LIMIT_PROPERTY = "<MinLimit>k__BackingField";
        private const string MAX_LIMIT_PROPERTY = "<MaxLimit>k__BackingField";
        private const string MIN_VALUE_PROPERTY = "m_MinValue";
        private const string MAX_VALUE_PROPERTY = "m_MaxValue";

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var minValueProp = property.FindPropertyRelative(MIN_VALUE_PROPERTY);
            var maxValueProp = property.FindPropertyRelative(MAX_VALUE_PROPERTY);
            var minLimitProp = property.FindPropertyRelative(MIN_LIMIT_PROPERTY);
            var maxLimitProp = property.FindPropertyRelative(MAX_LIMIT_PROPERTY);

            var minLimit = minLimitProp.floatValue;
            var maxLimit = maxLimitProp.floatValue;
            var minValue = minValueProp.floatValue;
            var maxValue = maxValueProp.floatValue;

            EditorGUI.BeginProperty(rect, label, property);
            EditorGUI.BeginProperty(rect, label, property);

            var indentLength = NaughtyEditorGUI.GetIndentLength(rect);
            var labelWidth = EditorGUIUtility.labelWidth + NaughtyEditorGUI.HorizontalSpacing;
            var floatFieldWidth = EditorGUIUtility.fieldWidth;
            var sliderWidth = rect.width - labelWidth - 2.0f * floatFieldWidth;
            var sliderPadding = 5.0f;

            var labelRect = new Rect(
                rect.x,
                rect.y,
                labelWidth,
                rect.height);

            var sliderRect = new Rect(
                rect.x + labelWidth + floatFieldWidth + sliderPadding - indentLength,
                rect.y,
                sliderWidth - 2.0f * sliderPadding + indentLength,
                rect.height);

            var minFloatFieldRect = new Rect(
                rect.x + labelWidth - indentLength,
                rect.y,
                floatFieldWidth + indentLength,
                rect.height);

            var maxFloatFieldRect = new Rect(
                rect.x + labelWidth + floatFieldWidth + sliderWidth - indentLength,
                rect.y,
                floatFieldWidth + indentLength,
                rect.height);

            // Draw the label
            EditorGUI.LabelField(labelRect, label.text);

            // Draw the slider
            EditorGUI.BeginChangeCheck();

            //Vector2 sliderValue = property.vector2Value;

            EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, 
                minLimit, maxLimit);

            minValue = EditorGUI.FloatField(minFloatFieldRect, minValue);
            minValue = Mathf.Clamp(minValue, minLimit, Mathf.Min(maxLimit, maxValue));

            maxValue = EditorGUI.FloatField(maxFloatFieldRect, maxValue);
            maxValue = Mathf.Clamp(maxValue, Mathf.Max(minLimit, minValue), maxLimit);

            if (EditorGUI.EndChangeCheck())
            {
                //property.vector2Value = sliderValue;

                minValueProp.floatValue = minValue;
                maxValueProp.floatValue = maxValue;

                minLimitProp.floatValue = minLimit;
                maxLimitProp.floatValue = maxLimit;

                //property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
            EditorGUI.EndProperty();
        }
    }
}