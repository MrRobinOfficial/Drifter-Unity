using System.Linq;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using Drifter.Attributes;

namespace Drifter.Editor
{
    [CustomPropertyDrawer(typeof(GroupAttribute))]
    public class GroupDrawer : DecoratorDrawer
    {
        public const string headerLabelClassName = "unity-header-drawer__label";

        private bool foldout = true;

        public override void OnGUI(Rect position)
        {
            position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;
            position = EditorGUI.IndentedRect(position);

            var style = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = new GUIStyleState()
                {
                    textColor = Color.yellow,
                },
            };

            GUI.Label(position, (attribute as GroupAttribute).name, style);
            foldout = EditorGUI.Foldout(position, foldout, GUIContent.none);
        }

        public override float GetHeight()
        {
            var content = new GUIContent((attribute as GroupAttribute).name);
            var fullTextHeight = EditorStyles.boldLabel.CalcHeight(content, 1.0f);

            var lines = 1;

            if ((attribute as GroupAttribute).name != null)
                lines = (attribute as GroupAttribute).name.Count(a => a == '\n') + 1;

            var eachLineHeight = fullTextHeight / lines;
            return EditorGUIUtility.singleLineHeight * 1.5f + (eachLineHeight * (lines - 1));
        }

        //public override VisualElement CreatePropertyGUI()
        //{
        //    var text = (attribute as GroupAttribute).name;
        //    var label = new Label(text);
        //    label.AddToClassList(headerLabelClassName);
        //    return label;
        //}
    }
}