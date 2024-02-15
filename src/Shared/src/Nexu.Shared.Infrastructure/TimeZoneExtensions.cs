using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime.TimeZones;

namespace Nexu.Shared.Infrastructure
{
    public static class TimeZoneExtensions
    {
        private static readonly HashSet<string> Ids = GetTimeZoneIds();

        public static bool IsTimeZone(this string s)
        {
            return !string.IsNullOrEmpty(s) && Ids.Contains(s);
        }

        private static HashSet<string> GetTimeZoneIds()
        {

            var defaultZoneLocations = TzdbDateTimeZoneSource.Default
                .ZoneLocations;

            if (defaultZoneLocations == null)
            {
                throw new InvalidOperationException();
            }

            return new HashSet<string>(defaultZoneLocations
                .Select(x => x.ZoneId));
        }
    }
}
