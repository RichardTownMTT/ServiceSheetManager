using ServiceSheetManager.Models;
using ServiceSheetManager.Models.Helpers;
using ServiceSheetManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
                    ServiceSheetApprovalIndexVM vmItem = new ServiceSheetApprovalIndexVM();
                    vmItem.Id = canvasSheet.Id;
                    vmItem.Approved = canvasSheet.Approved;
                    vmItem.Customer = canvasSheet.Customer;
                    vmItem.FirstName = canvasSheet.UserFirstName;
                    vmItem.JobStart = canvasSheet.DtJobStart;
                    vmItem.MachineMake = canvasSheet.MachineMakeModel;
                    vmItem.MachineSerial = canvasSheet.MachineSerial;
                    vmItem.MttJobNumber = canvasSheet.MttJobNumber;
                    vmItem.SubmissionNumber = canvasSheet.SubmissionNumber;
                    vmItem.Surname = canvasSheet.UserSurname;
                    vmItem.Username = canvasSheet.Username;

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

            //return the view

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Approve(ServiceSheetApprovalVM vm)
        {
            //Validate entities

            bool errors = false;

            bool valid = TryValidateModel(vm.ServiceSheetModel);
            if (!valid)
            {
                errors = true;
            }


            foreach (var day in vm.ServiceDayModels)
            {
                bool validDay = TryValidateModel(day);
                if(!validDay)
                {
                    errors = true;
                }
            }

            if(errors)
            {
                //Output errors
                vm.Errors = errors;
                return View(vm);
            }

            //Mark canvas sheet as approved
            var canvasEntities = db.CanvasRawDatas.Where(c => c.SubmissionNumber == vm.ServiceSheetModel.SubmissionNumber);
            foreach (var item in canvasEntities)
            {
                item.Approved = true;
            }
            
            var serviceSheetAdd = vm.ServiceSheetModel;

            List<ServiceDay> serviceDaysAdd = new List<ServiceDay>();

            foreach (var day in vm.ServiceDayModels)
            {
                day.ServiceDayEntity.ServiceSheet = serviceSheetAdd;
                serviceDaysAdd.Add(day.ServiceDayEntity);
            }

            serviceSheetAdd.ServiceDays = serviceDaysAdd;
            
            db.ServiceSheets.Add(serviceSheetAdd);
            var serviceDaysToSave = vm.ServiceDayModels.Select(m => m.ServiceDayEntity).ToList();
            db.ServiceDays.AddRange(serviceDaysToSave);

            //db.Entry(serviceSheetAdd).State = EntityState.Added;
            foreach (var daySave in serviceDaysAdd)
            {
                daySave.ServiceSheet = serviceSheetAdd;
                //db.Entry(daySave).State = EntityState.Added;
            }
            
            try
            {
                await db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error occured while saving: " + ex.ToString());
                return View(vm);
            }

            return View(vm);
        }
    }
}