#if UNITY_EDITOR

using UnityEditor;
using static Drifter.Utility.MathUtility;

namespace Drifter.Extensions
{
    /// <summary>
    /// <a href="https://en.wikivoyage.org/wiki/Metric_and_Imperial_equivalents"></a>
    /// </summary>
    public enum UnitType : byte
    {
        Metric,
        Imperial,
    }

    public static partial class DrifterEditorExtensions
    {
        private const string UNIT_HASH = "editor-unit";

        public static UnitType Unit
        {
            get => (UnitType)EditorPrefs.GetInt(UNIT_HASH, (int)UnitType.Metric);
            set
            {
                EditorPrefs.SetInt(UNIT_HASH, (int)value);

                //UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

                //foreach (var editor in ActiveEditorTracker.sharedTracker.activeEditors)
                //{
                //    editor.Repaint();
                //    editor.serializedObject.Update();
                //}
            }
        }

        [MenuItem("Tools/Drifter/Switch To Metric", priority = -100050)]
        private static void SwitchToMetric() => Unit = UnitType.Metric;

        [MenuItem("Tools/Drifter/Switch To Imperial", priority = -100050)]
        private static void SwitchToImperial() => Unit = UnitType.Imperial;

        [MenuItem("Tools/Drifter/Switch To Metric", isValidateFunction: true, priority = -100050)]
        private static bool CheckUnitMetric() => Unit != UnitType.Metric;

        [MenuItem("Tools/Drifter/Switch To Imperial", isValidateFunction: true, priority = -100050)]
        private static bool CheckUnitImperial() => Unit != UnitType.Imperial;

        public static float AsMass(this float value) => Unit switch
        {
            UnitType.Imperial => Round(value * 2.205f),
            UnitType.Metric => value,
            _ => value,
        };

        public static float AsLength(this float value) => Unit switch
        {
            UnitType.Imperial => Round(value * 1.09361f),
            UnitType.Metric => value,
            _ => value,
        };

        public static string GetMassUnit() => Unit switch
        {
            UnitType.Metric => "[kg]",
            UnitType.Imperial => "[lbs]",
            _ => "[kg]",
        };

        public static string GetLengthUnit() => Unit switch
        {
            UnitType.Metric => "[m]",
            UnitType.Imperial => "[yds]",
            _ => "[m]",
        };
    }
}

#endif