using ServiceSheetManager.Models.NonDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceSheetManager.ViewModels
{
    public class EngineerCalendarVM
    {
        public EngineerCalendarVM()
        {
            CalendarDays = new Dictionary<DateTime, EngineerCalendarDay>();
        }

        public SelectList WeeksList { get; set; }
        public Dictionary<DateTime, EngineerCalendarDay> CalendarDays { get; set; }
        public String SelectedWeek { get; set; }
    }
}