#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UIElements;

namespace Drifter.Attributes
{
    public class ExposedDataAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(ExposedDataAttribute))]
    public class ExposedDataAtttributeDrawer : PropertyDrawer
    {
        private Editor editor = null;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = base.CreatePropertyGUI(property);

            //EditorGUI.PropertyField(position, property, label, includeChildren: true);

            if (property.objectReferenceValue == null)
                return root;

            //property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                if (editor == null)
                    Editor.CreateCachedEditor(property.objectReferenceValue, editorType: null, ref editor);

                if (editor != null)
                    editor.OnInspectorGUI();

                EditorGUI.indentLevel--;
            }

            return root;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, includeChildren: true);

            if (property.objectReferenceValue == null)
                return;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                if (editor == null)
                    Editor.CreateCachedEditor(property.objectReferenceValue, editorType: null, ref editor);

                if (editor != null)
                    editor.OnInspectorGUI();

                EditorGUI.indentLevel--;
            }
        }
    }
}