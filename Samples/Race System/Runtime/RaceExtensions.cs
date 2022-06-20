using System;

namespace Drifter.Samples.RaceSystem
{
    public static class RaceExtensions
    {
        public static string AsString(this TimeSpan span) => span.ToString(@"hh\:mm\:ss\.ffff");
    }
}