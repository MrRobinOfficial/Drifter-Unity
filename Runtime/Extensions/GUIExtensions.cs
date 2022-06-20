using UnityEngine;

namespace Drifter.Extensions
{
    public static class GUIExtensions
    {
        private const float MIN_WIDTH = 150f;

        public static void DrawLabel(string text, GUIStyle style) => GUILayout.Label(text, style, GUILayout.ExpandWidth(expand: false));
    }
}