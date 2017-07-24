using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels.Helpers
{
    public class JobCostVmHelper
    {
        private List<ServiceSheet> sheets;
        public int NumberEngineers { get; set; }
        public int TotalDays { get; set; }
        public double TotalHoursOnsite { get; set; }
        public double TotalTravelTime { get; set; }
        public int TotalDailyAllowances { get; set; }
        public int TotalOvernightAllowances { get; set; }
        public int TotalMileage { get; set; }
        public double StandardHours { get; set; }
        public double OvertimeHours { get; set; }

        public JobCostVmHelper(List<ServiceSheet> sheets)
        {
            this.sheets = sheets;
        }

        public bool CalculateRates()
        {
            if (sheets.Count == 0)
            {
                return false;
            }

            NumberEngineers = sheets.Select(e => e.Username).Distinct().Count();
            TotalDays = sheets.SelectMany(m => m.ServiceDays).Count();
            TotalHoursOnsite = sheets.Sum(h => h.JobTotalTimeOnsite);
            TotalTravelTime = sheets.Sum(t => t.JobTotalTravelTime);
            TotalDailyAllowances = sheets.Sum(d => d.TotalDailyAllowances);
            TotalOvernightAllowances = sheets.Sum(o => o.TotalOvernightAllowances);
            TotalMileage = sheets.Sum(m => m.JobTotalMileage);

            foreach (var sheet in sheets)
            {
                List<ServiceDay> days = sheet.ServiceDays.ToList();
                foreach (var day in days)
                {
                    double hours = day.TotalOnsiteTime;
                    switch (day.DtReport.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            if (hours > 8)
                            {
                                StandardHours += 8;
                                OvertimeHours += hours - 8;
                            }
                            else
                            {
                                StandardHours += hours;
                            }
                            break;
                        case DayOfWeek.Tuesday:
                            if (hours > 8)
                            {
                                StandardHours += 8;
                                OvertimeHours += hours - 8;
                            }
                            else
                            {
                                StandardHours += hours;
                            }
                            break;
                        case DayOfWeek.Wednesday:
                            if (hours > 8)
                            {
                                StandardHours += 8;
                                OvertimeHours += hours - 8;
                            }
                            else
                            {
                                StandardHours += hours;
                            }
                            break;
                        case DayOfWeek.Thursday:
                            if (hours > 8)
                            {
                                StandardHours += 8;
                                OvertimeHours += hours - 8;
                            }
                            else
                            {
                                StandardHours += hours;
                            }
                            break;
                        case DayOfWeek.Friday:
                            if (hours > 6)
                            {
                                StandardHours += 6;
                                OvertimeHours += hours - 6;
                            }
                            else
                            {
                                StandardHours += hours;
                            }
                            break;
                        case DayOfWeek.Saturday:
                            StandardHours += 0;
                            OvertimeHours += hours;
                            break;
                        case DayOfWeek.Sunday:
                            StandardHours += 0;
                            OvertimeHours += hours;
                            break;
                        default:
                            throw new Exception("Unknown day");
                    }

                }
            }

            return true;
        }
    }
}