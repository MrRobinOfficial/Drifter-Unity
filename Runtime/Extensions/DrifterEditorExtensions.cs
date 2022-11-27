#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using static Drifter.Extensions.DrifterExtensions;
using static Drifter.Utility.MathUtility;

namespace Drifter.Extensions
{
    public static partial class DrifterEditorExtensions
    {
        public const string HELP_URL = "https://github.com/MrRobinftw/Drifter/wiki";

        public static SerializedProperty FindPropertyByAutoPropertyName(this SerializedObject obj, 
            string propName) => obj.FindProperty(GetAutoPropertyName(propName));

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

        public static void DrawRay(Vector3 origin, Vector3 direction, Color color, float scale = 1f, float scaleMin = -1f, float scaleMax = 1f)
        {
            const float THICKNESS = 5f;

            var scaleNorm = RemapClamped(scale, inA: scaleMin, inB: scaleMax, outA: -1f, outB: 1f);

            Handles.color = color;
            Handles.DrawLine(origin, origin + direction * scaleNorm, THICKNESS);
        }
    }
}

#endif