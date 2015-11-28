using System;

namespace Sharpen
{
    internal static class Extensions
    {
        private static readonly long EPOCH_TICKS = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        public static T ValueOf<T>(T val)
        {
            return val;
        }
        public static int ValueOf(string val)
        {
            return int.Parse(val.ToString());
        }

        public static long ToMillisecondsSinceEpoch(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("dateTime is expected to be expressed as a UTC DateTime", "dateTime");
            }
            return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), TimeSpan.Zero).ToMillisecondsSinceEpoch();
        }

        public static long ToMillisecondsSinceEpoch(this DateTimeOffset dateTimeOffset)
        {
            return (((dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks) - EPOCH_TICKS) / TimeSpan.TicksPerMillisecond);
        }

        public static string ToHexString(int val)
        {
            return Convert.ToString(val, 16);
        }
    }
}