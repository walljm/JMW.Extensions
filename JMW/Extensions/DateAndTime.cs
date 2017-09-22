using System;

namespace JMW.Extensions
{
    public static class DateAndTime
    {
        public static string GetElapsedTime(this DateTime curr, DateTime time)
        {
            return new TimeSpan(curr.Ticks - time.Ticks).ToString(@"hh\:mm\:ss");
        }
    }
}