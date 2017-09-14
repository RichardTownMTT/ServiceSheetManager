using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModels;
using ServiceSheetManager.Helpers;

namespace ServiceSheetManager.Controllers
{
    public class JobCostController : Controller
    {
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        // GET: JobCost
        // GET: JobCost
        public async Task<ActionResult> Index([Bind] JobCostDetailsVM retval)
        {
            JobCostDetailsVM costDetails = new JobCostDetailsVM();

            //Copy the required values from the returned values
            if (retval.ActiveJobDateFrom != new DateTime())
            {
                costDetails.ActiveJobDateFrom = retval.ActiveJobDateFrom;
            }
            else
            {
                costDetails.ActiveJobDateFrom = DateTime.Now.AddDays(-7);
            }

            if (retval.ActiveJobDateTo != new DateTime())
            {
                costDetails.ActiveJobDateTo = retval.ActiveJobDateTo;
            }
            else
            {
                costDetails.ActiveJobDateTo = DateTime.Now;
            }

            if (!string.IsNullOrEmpty(retval.SelectedJobNumber))
            {
                costDetails.SelectedJobNumber = retval.SelectedJobNumber;
            }



            //Create select list of all job numbers
            var jobNumbers = await db.ServiceSheets.Where(s => s.DtJobStart > costDetails.ActiveJobDateFrom)
                .Where(s => s.DtJobStart <= costDetails.ActiveJobDateTo).GroupBy(s => s.MttJobNumber).ToListAsync();

            var allJobNumbers = new List<SelectListItem>();

            foreach (var number in jobNumbers)
            {
                allJobNumbers.Add(new SelectListItem { Text = number.Key, Value = number.Key });
            }


            if (allJobNumbers.Count == 0)
            {
                allJobNumbers.Add(new SelectListItem { Text = "Invalid", Value = "Select valid dates" });
            }

            costDetails.AllJobNumbers = allJobNumbers;


            if (!string.IsNullOrEmpty(retval.SelectedJobNumber))
            {
                var sheets = await db.ServiceSheets.Where(m => m.MttJobNumber.Equals(retval.SelectedJobNumber))
                    .Include(m => m.ServiceDays)
                    .ToListAsync();

                JobCostVmHelper costingHelper = new JobCostVmHelper(sheets);
                bool success = costingHelper.CalculateRates();

                if (success)
                {
                    costDetails.NumberEngineers = costingHelper.NumberEngineers;
                    costDetails.TotalHoursOnsite = costingHelper.TotalHoursOnsite;
                    costDetails.TotalTravelTime = costingHelper.TotalTravelTime;
                    costDetails.TotalDays = costingHelper.TotalDays;
                    costDetails.TotalDailyAllowances = costingHelper.TotalDailyAllowances;
                    costDetails.TotalOvernightAllowances = costingHelper.TotalOvernightAllowances;
                    costDetails.TotalMileage = costingHelper.TotalMileage;
                    costDetails.StandardHours = costingHelper.StandardHours;
                    costDetails.OvertimeHours = costingHelper.OvertimeHours;
                }
            }

            return View(costDetails);
        }
    }
}
