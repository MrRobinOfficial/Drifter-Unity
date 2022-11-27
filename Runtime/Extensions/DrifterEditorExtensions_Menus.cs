#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Drifter.Extensions
{
    public static partial class DrifterEditorExtensions
    {
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
    } 
}

#endif