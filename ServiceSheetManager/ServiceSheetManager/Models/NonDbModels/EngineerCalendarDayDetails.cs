using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.Models.NonDbModels
{
    public class EngineerCalendarDayDetails
    {
        public EngineerCalendarDayDetails()
        {
            JobsList = new List<JobDetails>();
        }

        public string Engineer { get; set; }
        public double TotalHours { get; set; }
        public List<JobDetails> JobsList { get; set; }
        public bool MissingDay { get; set; }
        public string EngineerInitials { get; set; }
    }
}