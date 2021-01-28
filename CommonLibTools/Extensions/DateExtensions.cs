using System;

namespace CommonLibTools.Extensions
{
    public static class DateExtensions
    {

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime StartOfWeek(this DateTime dt)
        {
            int diff = dt.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
        

        public static DateTimeOffset? ToDate(this string date)
        {
            try
            {
                var dat = DateTimeOffset.Parse(date);
                return dat;
            }
            catch (Exception)
            {

                return null;
            }
        }

    }
}
