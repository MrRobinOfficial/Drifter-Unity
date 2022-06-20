#define DRAW_DEFAULT
//#undef DRAW_DEFAULT

using UnityEditor;
using UnityEngine;

using static Drifter.Extensions.DrifterEditorExtensions;

namespace Drifter.Editor.Editors
{
    [CustomEditor(typeof(BaseVehicle), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class BaseVehicleEditor : NaughtyAttributes.Editor.NaughtyInspector
    {
        private string subPath = string.Empty;
        private SerializedProperty subPathProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            var vehicle = (BaseVehicle)target;

            subPath = vehicle.SubPath;
            subPathProperty = serializedObject.FindProperty("m_SubPath");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Repaint();
        }

        public override void OnInspectorGUI()
        {
#if !DRAW_DEFAULT
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

            EditorGUILayout.Space();

            var vehicle = (BaseVehicle)target;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginDisabledGroup(disabled: true);
            EditorGUILayout.LabelField("License Plate", vehicle.LicensePlate);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(disabled: true);

            EditorGUILayout.LabelField("Sub Path", vehicle.SubPath);

            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            showVehicle = EditorGUILayout.Foldout(showVehicle, "Vehicle Properties");

            if (showVehicle)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.PropertyField(serializedObject.FindPropertyByAutoPropertyName(nameof(vehicle.Mass)));

                EditorGUILayout.PropertyField(serializedObject.FindPropertyByAutoPropertyName(nameof(vehicle.CenterOfMass)));

                EditorGUILayout.PropertyField(serializedObject.FindPropertyByAutoPropertyName(nameof(vehicle.Inertia)));

                EditorGUILayout.PropertyField(serializedObject.FindPropertyByAutoPropertyName(nameof(vehicle.DragCoefficient)));

                EditorGUILayout.PropertyField(serializedObject.FindPropertyByAutoPropertyName(nameof(vehicle.LiftCoefficient)));

                EditorGUILayout.PropertyField(serializedObject.FindPropertyByAutoPropertyName(nameof(vehicle.FrontalArea)));

                EditorGUILayout.EndVertical();
            }

            DrawPropertiesExcluding(serializedObject,
                "m_Script",
                GetAutoPropertyName(nameof(vehicle.Mass)),
                GetAutoPropertyName(nameof(vehicle.CenterOfMass)),
                GetAutoPropertyName(nameof(vehicle.Inertia)),
                GetAutoPropertyName(nameof(vehicle.DragCoefficient)),
                GetAutoPropertyName(nameof(vehicle.LiftCoefficient)),
                GetAutoPropertyName(nameof(vehicle.FrontalArea)));

            serializedObject.ApplyModifiedProperties();
#else
            base.OnInspectorGUI();
#endif
        }

        private bool showVehicle = false;
    }
}