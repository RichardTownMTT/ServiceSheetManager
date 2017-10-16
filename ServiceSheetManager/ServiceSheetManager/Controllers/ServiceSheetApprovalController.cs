using ServiceSheetManager.Helpers;
using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ServiceSheetManager.Controllers
{
    public class ServiceSheetApprovalController : Controller
    {
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        // GET: ServiceSheetApproval
        public async Task<ActionResult> Index()
        {
            //Load all canvas entities that haven't been approved
            List<CanvasRawData> canvasEntitiesToApprove = await db.CanvasRawDatas.Where(c => c.Approved == false).ToListAsync();

            List<ServiceSheetApprovalIndexVM> sheetVM = new List<ServiceSheetApprovalIndexVM>();

            foreach (var canvasSheet in canvasEntitiesToApprove)
            {
                int submissionNumber = canvasSheet.SubmissionNumber;

                if (sheetVM.Where(m => m.SubmissionNumber == submissionNumber).FirstOrDefault() == null)
                {
                    ServiceSheetApprovalIndexVM vmItem = new ServiceSheetApprovalIndexVM
                    {
                        Id = canvasSheet.Id,
                        Approved = canvasSheet.Approved,
                        Customer = canvasSheet.Customer,
                        FirstName = canvasSheet.UserFirstName,
                        JobStart = canvasSheet.DtJobStart,
                        MachineMake = canvasSheet.MachineMakeModel,
                        MachineSerial = canvasSheet.MachineSerial,
                        MttJobNumber = canvasSheet.MttJobNumber,
                        SubmissionNumber = canvasSheet.SubmissionNumber,
                        Surname = canvasSheet.UserSurname,
                        Username = canvasSheet.Username
                    };

                    sheetVM.Add(vmItem);
                }
            }
            //Translate to service sheet / day entities

            return View(sheetVM);
        }

        public async Task<ActionResult> Approve(int SubmissionNo)
        {
            //Load the canvas submission
            List<CanvasRawData> canvasEntities = await db.CanvasRawDatas.Where(c => c.SubmissionNumber == SubmissionNo).ToListAsync();
            if (canvasEntities == null)
            {
                Response.Redirect("Error");
            }

            //Create the service sheet and day entities
            ServiceSheetApprovalVM vm = CanvasRawDataHelper.CreateServiceEntitiesForCanvasEntities(canvasEntities);

            //Load the images
            vm.ServiceSheetModel.LoadCanvasImages();

            //return the view

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Approve(ServiceSheetApprovalVM vm)
        {
            //Validate entities

            if (!ModelState.IsValid)
            {
                vm.Errors = true;
                return View(vm);
            }

            //Mark canvas sheet as approved
            var canvasEntities = db.CanvasRawDatas.Where(c => c.SubmissionNumber == vm.ServiceSheetModel.SubmissionNumber);
            foreach (var item in canvasEntities)
            {
                item.Approved = true;
            }

            //Create a service sheet entitiy
            ServiceSheet sheetToSave = ServiceSheetVM.CreateServiceSheetFromVM(vm);

            //Create service day entitites
            List<ServiceDay> daysToSave = ServiceDayVM.CreateServiceDaysFromVM(vm, sheetToSave);
            sheetToSave.ServiceDays = daysToSave;
            
            //Update times for days.  Might not be correct if javascript is disabled on client
            foreach (var day in daysToSave)
            {
                //Verify that the totals, dates on the travel times, etc. are correct
                UpdateServiceDayTimes(day);
            }
            //The mileage, allowance and time totals need to be updated on the service sheet
            ServiceSheetHelpers.UpdateServiceSheetTotals(sheetToSave);


            //Save
            //Add the entities to save to the db context
            db.ServiceSheets.Add(sheetToSave);
            db.ServiceDays.AddRange(daysToSave);

            try
            {
                await db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error occured while saving: " + ex.ToString());
                return RedirectToAction("Error");
            }

            return RedirectToAction("Index");
        }

        //private void updateServiceSheetTotals(ServiceSheet serviceSheetAdd)
        //{
        //    //Update all the totals held on the service sheet
        //    int totalMileage = 0;
        //    double totalTimeOnsite = 0;
        //    double totalTravelTime = 0;
        //    int totalDailyAllowances = 0;
        //    int totalOvernightAllowances = 0;
        //    int totalBarrierPayments = 0;

        //    foreach (var day in serviceSheetAdd.ServiceDays)
        //    {
        //        totalMileage += day.Mileage;
        //        totalTimeOnsite += day.TotalOnsiteTime;
        //        totalTravelTime += day.TotalTravelTime;
        //        totalDailyAllowances += day.DailyAllowance;
        //        totalOvernightAllowances += day.OvernightAllowance;
        //        totalBarrierPayments += day.BarrierPayment;
        //    }

        //    serviceSheetAdd.JobTotalMileage = totalMileage;
        //    serviceSheetAdd.JobTotalTimeOnsite = totalTimeOnsite;
        //    serviceSheetAdd.JobTotalTravelTime = totalTravelTime;
        //    serviceSheetAdd.TotalDailyAllowances = totalDailyAllowances;
        //    serviceSheetAdd.TotalOvernightAllowances = totalOvernightAllowances;
        //    serviceSheetAdd.TotalBarrierPayments = totalBarrierPayments;
        //}

        private void UpdateServiceDayTimes(ServiceDay day)
        {
            //The travel times, etc will have todays date on, need to set to the dtReport date
            DateTime reportDate = day.DtReport;
            TimeSpan travelStartTime = day.TravelStartTime.TimeOfDay;
            DateTime travelStart = reportDate.Date + travelStartTime;
            day.TravelStartTime = travelStart;

            TimeSpan arrivalOnsiteTime = day.ArrivalOnsiteTime.TimeOfDay;
            DateTime arrivalOnsite = reportDate.Date + arrivalOnsiteTime;
            day.ArrivalOnsiteTime = arrivalOnsite;

            TimeSpan departureTime = day.DepartureSiteTime.TimeOfDay;
            DateTime departureSite = reportDate.Date + departureTime;
            day.DepartureSiteTime = departureSite;

            TimeSpan travelEndTime = day.TravelEndTime.TimeOfDay;
            DateTime travelEnd = reportDate.Date + travelEndTime;
            day.TravelEndTime = travelEnd;

            //The onsite and travel times now need to be recalculated
            TimeSpan travelToSiteTime = arrivalOnsite - travelStart;
            day.TravelToSiteTime = travelToSiteTime.TotalHours;

            TimeSpan totalOnsiteTime = departureSite - arrivalOnsite;
            day.TotalOnsiteTime = totalOnsiteTime.TotalHours;

            TimeSpan travelFromSiteTime = travelEnd - departureSite;
            day.TravelFromSiteTime = travelFromSiteTime.TotalHours;

            day.TotalTravelTime = travelToSiteTime.TotalHours + travelFromSiteTime.TotalHours;
        }
    }
}