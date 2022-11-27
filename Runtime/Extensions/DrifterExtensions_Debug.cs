using UnityEngine;

namespace Drifter.Extensions
{
    public enum PrintType : byte
    {
        Log,
        Important,
        Error,
        Warning,
    }

    public static partial class DrifterExtensions
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Print(string message, PrintType type, Object ctx = null)
        {
            var logType = Debug.unityLogger.filterLogType;
            var stackTrace = Application.GetStackTraceLogType(logType);

            Application.SetStackTraceLogType(logType, StackTraceLogType.None);

            var builder = new System.Text.StringBuilder();
            builder.Append($"<color={GetColor()}>");

            if (type == PrintType.Error || type == PrintType.Important)
                builder.Append("<b>");

            builder.Append(message);

            if (type == PrintType.Error || type == PrintType.Important)
                builder.Append("</b>");

            builder.Append("</color>");

            Debug.Log(builder.ToString(), ctx);

            Application.SetStackTraceLogType(logType, stackTrace);

            string GetColor() => type switch
            {
                PrintType.Log => "#55ff00",
                PrintType.Error => "#ff0000",
                PrintType.Warning => "#ffd500",
                PrintType.Important => "#00ffff",
                _ => "#ffffff",
            };
        }
    }
}