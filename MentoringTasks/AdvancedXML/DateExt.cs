using System;
using System.Globalization;

namespace AdvancedXML
{
    public class DateExt
    {
        public string GetPubDate(DateTime date)
        {
            var pubDate = $"{date.DayOfWeek.ToString().Substring(0, 3)}, ";
            pubDate += $"{date.Day} ";
            pubDate += $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month)} ";
            pubDate += $"{date.Year} ";
            pubDate += $"{date.TimeOfDay} -0700";
            return pubDate;
        }
    }
}