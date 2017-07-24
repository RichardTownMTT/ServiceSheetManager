using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceSheetManager.ViewModels
{
    public class JobCostDetailsVM
    {
        public JobCostDetailsVM()
        {
            //ServiceSheets = new List<ServiceSheet>();
            SelectedJobNumber = "";
            AllJobNumbers = new List<SelectListItem>();
            NumberEngineers = 0;
            TotalDays = 0;
            TotalHoursOnsite = 0;
            TotalTravelTime = 0;
            TotalDailyAllowances = 0;
            TotalOvernightAllowances = 0;
            TotalMileage = 0;
            StandardHours = 0;
        }

        public List<SelectListItem> AllJobNumbers { get; set; }
        public string SelectedJobNumber { get; set; }
        //From and to dates for the active job selector
        [Display(Name = "Active jobs from")]
        [DataType(DataType.Date)]
        public DateTime ActiveJobDateFrom { get; set; }

        [Display(Name = "Active jobs from")]
        [DataType(DataType.Date)]
        public DateTime ActiveJobDateTo { get; set; }

        public int NumberEngineers { get; set; }
        public int TotalDays { get; set; }
        public double TotalHoursOnsite { get; set; }
        public double TotalTravelTime { get; set; }
        public int TotalDailyAllowances { get; set; }
        public int TotalOvernightAllowances { get; set; }
        public int TotalMileage { get; set; }
        public double StandardHours { get; set; }
        public double OvertimeHours { get; set; }
    }
}
