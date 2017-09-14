using ServiceSheetManager.Models.NonDbModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.Helpers
{
    public class WeekNumberCreator
    {
        public static Dictionary<String, WeekNumber> CreateWeekNumberDictionary(DateTime startDate, DateTime endDate)
        {
            Dictionary<string, WeekNumber> retval = new Dictionary<string, WeekNumber>();

            DateTime currentDate = startDate;
            while (currentDate <= endDate)
            {
                var weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                int year = currentDate.Year;

                string key = weekNumber.ToString() + " - " + year.ToString();

                if (!retval.ContainsKey(key))
                {
                    WeekNumber wkNumber = new WeekNumber();
                    wkNumber.WeekNo = weekNumber;
                    wkNumber.Year = year;
                    var dayOfWeek = currentDate.DayOfWeek;

                    DateTime weekStartDate = currentDate.AddDays(-(int)dayOfWeek + 1);
                    DateTime weekEnd = weekStartDate.AddDays(7);
                    wkNumber.StartDate = weekStartDate;
                    wkNumber.EndDate = weekEnd;

                    retval.Add(key, wkNumber);
                }

                currentDate = currentDate.AddDays(7);
            }
            return retval;
        }
    }
}
