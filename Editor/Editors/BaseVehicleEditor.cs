using UnityEditor;
using UnityEngine;

namespace Drifter.Editor.Editors
{
    [CustomEditor(typeof(BaseVehicle), editorForChildClasses: true), CanEditMultipleObjects]
    public class BaseVehicleEditor : NaughtyAttributes.Editor.NaughtyInspector
    {
        private Rigidbody rigidbody;

        protected override void OnEnable()
        {
            base.OnEnable();

            var vehicle = (BaseVehicle)target;
            rigidbody = vehicle.Rigidbody;

            if (rigidbody != null)
                rigidbody.hideFlags = HideFlags.NotEditable;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (target == null && rigidbody != null)
                rigidbody.hideFlags = HideFlags.None;
        }

        public override void OnInspectorGUI()
        {
            var vehicle = (BaseVehicle)target;

            var idStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                wordWrap = true,
            };

            EditorGUI.BeginDisabledGroup(disabled: true);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField(label: "ID", label2: vehicle.ID, idStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}