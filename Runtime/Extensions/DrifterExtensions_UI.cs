using UnityEngine;

namespace Drifter.Extensions
{
    public static partial class DrifterExtensions
    {
        public static float Lerp(this float value, ref float oldValue) =>
            oldValue = Mathf.Lerp(oldValue, value, Time.unscaledDeltaTime);
    }
}