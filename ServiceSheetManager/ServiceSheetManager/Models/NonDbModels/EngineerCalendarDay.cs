using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.Models.NonDbModels
{
    public class EngineerCalendarDay
    {
        public EngineerCalendarDay(DateTime DateForCalendar)
        {
            CalendarDayDetails = new Dictionary<String, EngineerCalendarDayDetails>();
            this.CalendarDate = DateForCalendar;
        }

        public DateTime CalendarDate { get; set; }
        public Dictionary<String, EngineerCalendarDayDetails> CalendarDayDetails { get; set; }
    }
}