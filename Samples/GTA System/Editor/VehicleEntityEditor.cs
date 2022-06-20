using UnityEditor;
using UnityEngine;

namespace Drifter.Samples.GTASystem
{
	[CustomEditor(typeof(VehicleEntity))]
	public class VehicleEntityEditor : UnityEditor.Editor
	{
        public override void OnInspectorGUI()
        {
            var entity = (VehicleEntity)target;
            var rect = EditorGUILayout.BeginVertical();
            rect.height = EditorGUIUtility.singleLineHeight;

            var maxHealth = serializedObject.FindProperty("m_StartingHealth").intValue;
            var health = Application.isPlaying ? entity.Health : maxHealth;

            EditorGUI.ProgressBar(rect, health / maxHealth, "Health");
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}