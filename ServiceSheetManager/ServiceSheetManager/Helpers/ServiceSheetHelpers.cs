using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.Helpers
{
    public class ServiceSheetHelpers
    {
        public static void UpdateServiceSheetTotals(ServiceSheet serviceSheetAdd)
        {
            //Update all the totals held on the service sheet
            int totalMileage = 0;
            double totalTimeOnsite = 0;
            double totalTravelTime = 0;
            int totalDailyAllowances = 0;
            int totalOvernightAllowances = 0;
            int totalBarrierPayments = 0;

            foreach (var day in serviceSheetAdd.ServiceDays)
            {
                totalMileage += day.Mileage;
                totalTimeOnsite += day.TotalOnsiteTime;
                totalTravelTime += day.TotalTravelTime;
                totalDailyAllowances += day.DailyAllowance;
                totalOvernightAllowances += day.OvernightAllowance;
                totalBarrierPayments += day.BarrierPayment;
            }

            serviceSheetAdd.JobTotalMileage = totalMileage;
            serviceSheetAdd.JobTotalTimeOnsite = totalTimeOnsite;
            serviceSheetAdd.JobTotalTravelTime = totalTravelTime;
            serviceSheetAdd.TotalDailyAllowances = totalDailyAllowances;
            serviceSheetAdd.TotalOvernightAllowances = totalOvernightAllowances;
            serviceSheetAdd.TotalBarrierPayments = totalBarrierPayments;
        }
    }
}