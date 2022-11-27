using Drifter.Components;
using Syrus.Plugins.ChartEditor;
using UnityEditor;
using UnityEngine;

using UEditor = UnityEditor.Editor;

namespace Drifter.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(GearboxComponent))]
    public class GearboxComponentDrawer : PropertyDrawer
    {
        private const float SPACE = 10f;

        private const float RATIO = 16f / 9f;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            EditorGUI.PropertyField(position, property, includeChildren: true);

            //var width = position.width;
            //var height = position.width / RATIO;

            //var charRect = new Rect(position.position, size: new Vector2(width, height));

            //if (property.isExpanded)
            //{
            //    //position.y += SPACE;
            //    position.height = charRect.height;
            //    position.y += position.height;
            //}

            //if (!property.isExpanded)
            //    return;

            //EditorGUI.indentLevel++;

            //var gray = Color.gray;
            //gray.a = 0.5f;

            //GUIChartEditor.BeginChart(charRect, Color.black,
            //    GUIChartEditorOptions.ChartBounds(-0.5f, 1.5f, -0.25f, 1.25f),
            //    GUIChartEditorOptions.SetOrigin(ChartOrigins.BottomLeft),
            //    GUIChartEditorOptions.ShowGrid(cellWidth: 0.1f, cellHeight: 0.1f, gray)
            //);

            //GUIChartEditor.PushFunction(x => x * x * x, -0.5f, 1.5f, Color.green);
            //GUIChartEditor.EndChart();

            //EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    } 
}