using System;

namespace Utils
{
    public static class ServerTimeUtils
    {
        public static double MillisecondsToSeconds(long timestamp)
        {
            return TimeSpan.FromMilliseconds(timestamp).TotalSeconds;
        }

        public static DateTime TimeStampToDate(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
        }

        public static DateTimeOffset TimeStampToDateOffset(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
        }
    }
}