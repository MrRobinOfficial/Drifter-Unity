#if UNITY_EDITOR

#define DRAW_COLLIDER
#undef DRAW_COLLIDER

using UnityEngine;
using UnityEditor;
using Drifter.Components;

using static Drifter.Extensions.DrifterExtensions;
using static Drifter.Utility.DrifterMathUtility;

namespace Drifter.Extensions
{
    public static partial class DrifterEditorExtensions
    {
        public static string GetAutoPropertyName(string propName) => string.Format("<{0}>k__BackingField", propName);

        public static SerializedProperty FindPropertyByAutoPropertyName(this SerializedObject obj, string propName) => obj.FindProperty(string.Format("<{0}>k__BackingField", propName));

        public static bool IsSceneViewCameraInRange(Vector3 position, float distance)
        {
            var cameraPos = Camera.current.WorldToScreenPoint(position);

            return (cameraPos.x >= 0) &&
            (cameraPos.x <= Camera.current.pixelWidth) &&
            (cameraPos.y >= 0) &&
            (cameraPos.y <= Camera.current.pixelHeight) &&
            (cameraPos.z > 0) &&
            (cameraPos.z < distance);
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.NotInSelectionHierarchy)]
        private static void DrawWheelGizmo(WheelBehaviour obj, GizmoType gizmoTyp)
        {
            if (!Application.isPlaying)
            {
                DrawDisc();
                DrawCenterLine();
                DrawWidthLine();
                DrawDropLine();
            }

            const float THICKNESS = 1.5f;

            var vehicle = obj.Vehicle;

            if (Application.isPlaying && vehicle != null && vehicle.Body != null)
            {
                var FORCE_SCALE = vehicle.Body.mass * Mathf.Abs(Physics.gravity.y);

                DrawRay(obj.transform.position, obj.transform.up, Color.green, 
                    obj.NormalForce, -FORCE_SCALE, FORCE_SCALE);

                DrawRay(obj.transform.position, obj.transform.right, Color.red, 
                    obj.LateralForce, -FORCE_SCALE, FORCE_SCALE);

                DrawRay(obj.transform.position, obj.transform.forward, Color.blue, obj.LongitudinalForce, -FORCE_SCALE, FORCE_SCALE);
            }

#if DRAW_COLLIDER
            DrawDisc();
            DrawCenterLine();
            DrawWidthLine();
            DrawDropLine();
#endif

            void DrawDisc()
            {
                Handles.color = Color.green;
                Handles.DrawWireDisc(obj.GetWorldPosition(), obj.transform.right, obj.Radius, THICKNESS);
            }

            void DrawDropLine()
            {
                //Handles.color = Color.magenta;

                //var startPoint = GetOffsetPosition();
                //var endPoint = GetWorldPositon();

                //Handles.DrawLine(startPoint, endPoint);
            }

            void DrawCenterLine()
            {
                return;

                if (obj.Visual == null)
                    return;

                var visual = obj.Visual;
                Handles.color = Color.yellow;

                var length = obj.Radius * 0.8f;
                var dir = visual.forward;

                var startPoint = obj.GetWorldPosition() - dir * length;
                var endPoint = obj.GetWorldPosition() + dir * length;

                Handles.DrawLine(startPoint, endPoint, THICKNESS);
            }

            void DrawWidthLine()
            {
                Handles.color = Color.cyan;

                var length = obj.Width;
                var dir = obj.transform.right;

                var startPoint = obj.GetWorldPosition() - dir * length;
                var endPoint = obj.GetWorldPosition() + dir * length;

                Handles.DrawLine(startPoint, endPoint, THICKNESS);
            }
        }

        public static void DrawRay(Vector3 origin, Vector3 direction, Color color, float scale = 1f, float scaleMin = -1f, float scaleMax = 1f)
        {
            const float THICKNESS = 5f;

            var scaleNorm = RemapClamped(scale, inA: scaleMin, inB: scaleMax, outA: -1f, outB: 1f);

            Handles.color = color;
            Handles.DrawLine(origin, origin + direction * scaleNorm, THICKNESS);
        }

        [MenuItem("GameObject/Tools/Drifter/Vehicles/Create Car [Vehicle]", priority = -15001)]
        private static void CreateCarVehicle(MenuCommand menuCommand)
        {
            var go = new GameObject("Custom Game Object");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/Tools/Drifter/Vehicles/Create Bike [Vehicle]", priority = -15000)]
        private static void CreateBikeVehicle(MenuCommand menuCommand)
        {
            var go = new GameObject("Custom Game Object");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/Tools/Drifter/Lights/Low Beam [Light]", priority = -15003)]
        private static void CreateLowBeamLight(MenuCommand menuCommand)
        {
            var go = new GameObject("Custom Game Object");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/Tools/Drifter/Lights/High Beam [Light]", priority = -15002)]
        private static void CreateHighBeamLight(MenuCommand menuCommand)
        {
            var go = new GameObject("Custom Game Object");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("Tools/Drifter/Help/Docs...", priority = -1000100)]
        private static void VisitDocs() => Application.OpenURL(HELP_URL);

        [MenuItem("Tools/Drifter/Settings...", priority = -1000100)]
        private static void OpenSettings() => Application.OpenURL(HELP_URL);

        public const string HELP_URL = "https://github.com/MrRobinftw/Drifter/wiki";
    }
}

#endif