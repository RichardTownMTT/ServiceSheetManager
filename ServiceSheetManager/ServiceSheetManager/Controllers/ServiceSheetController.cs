using ServiceSheetManager.Models;
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
    public class ServiceSheetController : Controller
    {
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        // GET: ServiceSheet
        public async Task<ActionResult> Index([Bind] ServiceSheetIndexVM submittedVM)
        {
            ServiceSheetIndexVM vm = new ServiceSheetIndexVM();

            if(!ModelState.IsValid)
            {
                return View(submittedVM);
            }

            //Populate the list of engineers
            List<SelectListItem> engineerSL = await CreateEngineerList();
            vm.Engineers = engineerSL;

            List<ServiceSheet> serviceSheetsFound = new List<ServiceSheet>();

            //Customer search entered
            if (!String.IsNullOrEmpty(submittedVM.CustomerSearch))
            {
                serviceSheetsFound = await db.ServiceSheets.Where(s => s.Customer.Contains(submittedVM.CustomerSearch)).ToListAsync();
                vm.ServiceSheets = serviceSheetsFound;

                //Return the search term
                vm.CustomerSearch = submittedVM.CustomerSearch;
            }

            //Submission number search entered
            if (submittedVM.SubmissionNumber != 0)
            {
                serviceSheetsFound = await db.ServiceSheets.Where(s => s.SubmissionNumber == submittedVM.SubmissionNumber).ToListAsync();
                vm.ServiceSheets = serviceSheetsFound;

                //Return the search term
                vm.SubmissionNumber = submittedVM.SubmissionNumber;
            }

            //Job number search entered
            if (!String.IsNullOrEmpty(submittedVM.MttJobNumberSearch))
            {
                serviceSheetsFound = await db.ServiceSheets.Where(m => m.MttJobNumber.Contains(submittedVM.MttJobNumberSearch)).ToListAsync();
                vm.ServiceSheets = serviceSheetsFound;

                //Return the search term
                vm.MttJobNumberSearch = submittedVM.MttJobNumberSearch;
            }

            //Engineer search entered
            if (!string.IsNullOrEmpty(submittedVM.SelectedEngineer))
            {
                DateTime dateFrom = submittedVM.SheetsFromEngineerSearch;
                DateTime dateTo = submittedVM.SheetsToEngineerSearch;

                if (dateFrom > dateTo)
                {
                    ModelState.AddModelError(String.Empty, "The from date must be before the to date");
                    return View(vm);
                }

                serviceSheetsFound = await db.ServiceSheets.Where(e => e.Username.Equals(submittedVM.SelectedEngineer) 
                                            && e.ServiceDays.Any(sd => sd.DtReport >= dateFrom)
                                            && e.ServiceDays.Any(sd => sd.DtReport <= dateTo)).ToListAsync();
                vm.ServiceSheets = serviceSheetsFound;

                //Return the search terms
                vm.SelectedEngineer = submittedVM.SelectedEngineer;
                vm.SheetsFromEngineerSearch = submittedVM.SheetsFromEngineerSearch;
                vm.SheetsToEngineerSearch = submittedVM.SheetsToEngineerSearch;
            }

            //Date search entered
            if (submittedVM.SheetsFromDateSearch > new DateTime() && submittedVM.SheetsToDateSearch > new DateTime())
            {
                DateTime dateFrom = submittedVM.SheetsFromDateSearch;
                DateTime dateTo = submittedVM.SheetsToDateSearch;

                if (dateFrom > dateTo)
                {
                    ModelState.AddModelError(String.Empty, "The from date must be before the to date");
                    return View(vm);
                }

                serviceSheetsFound = await db.ServiceSheets.Where(d => d.ServiceDays.Any(s => s.DtReport >= dateFrom)
                                        && d.ServiceDays.Any(s => s.DtReport <= dateTo)).ToListAsync();
                vm.ServiceSheets = serviceSheetsFound;

                //Return the search terms
                vm.SheetsFromDateSearch = dateFrom;
                vm.SheetsToDateSearch = dateTo;
            }



            //else if (!string.IsNullOrEmpty(submittedVM.SelectedEngineer))
            //{
            //    var serviceSheetsEng = await db.ServiceSheets.Where(e => e.Username.Equals(submittedVM.SelectedEngineer)).ToListAsync();
            //    vm.ServiceSheets = serviceSheetsEng;
            //}
            //else if (!String.IsNullOrEmpty(submittedVM.SubmissionNumber.ToString()))
            //{
            //    var serviceSheetsSubmissionNumber = await db.ServiceSheets.Where(s => s.SubmissionNumber == submittedVM.SubmissionNumber).ToListAsync();
            //    vm.ServiceSheets = serviceSheetsSubmissionNumber;
            //}
            //else if (dateFrom != new DateTime() && dateTo != new DateTime())
            //{
            //    var serviceSheetDates = await db.ServiceSheets.Where(d => d.ServiceDays.Any(s => s.DtReport >= dateFrom)
            //                            && d.ServiceDays.Any(s => s.DtReport <= dateTo)).ToListAsync();
            //    vm.ServiceSheets = serviceSheetDates;
            //}
            //else
            //{
            //    //Default the date search to the last week
            //    DateTime currentDate = DateTime.Now;
            //    DateTime lastWeek = DateTime.Now.AddDays(-7);
            //    vm.SheetsFrom = lastWeek;
            //    vm.SheetsTo = currentDate;
            //    var serviceSheets = await db.ServiceSheets.OrderByDescending(s => s.SubmissionNumber).Take(5).ToListAsync();
            //    vm.ServiceSheets = serviceSheets;
            //}

            return View(vm);
        }

        private async Task<List<SelectListItem>> CreateEngineerList()
        {
            var engineers = await db.ServiceSheets.GroupBy(s => s.Username).Select(s => s.FirstOrDefault()).ToListAsync();

            List<SelectListItem> engineerSL = new List<SelectListItem>();
            //Add blank engineer
            engineerSL.Add(new SelectListItem() { Text = "", Value = "" });

            foreach (var engineer in engineers)
            {
                string engName = engineer.UserFirstName + " " + engineer.UserSurname;
                string username = engineer.Username;
                engineerSL.Add(new SelectListItem() { Text = engName, Value = username });
            }

            return engineerSL;
        }

        //Display
        public async Task<ActionResult> Display([Bind] int SubmissionNumber)
        {
            ServiceSheet sheet = await db.ServiceSheets.Where(s => s.SubmissionNumber.Equals(SubmissionNumber))
                .Include(s => s.ServiceDays).FirstOrDefaultAsync();

            return View(sheet);
        }
    }
}