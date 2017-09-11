using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using PagedList;

namespace ServiceSheetManager.Controllers
{
    public class ServiceSheetController : Controller
    {
        private const int pageSize = 10;
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        public ActionResult ListReports([Bind] int? page, int? submissionNumber, DateTime? sheetsFromDateSearch, DateTime? sheetsToDateSearch, string customerSearch,
                                            string mttJobNumberSearch, string selectedEngineer, string currentSortOrder)
        {
            int pageNumber = (page ?? 1);
            ServiceSheetListVM vm = new ServiceSheetListVM();

            List<ServiceSheet> serviceSheetsFound = GetServiceSheets(submissionNumber, sheetsFromDateSearch, sheetsToDateSearch, customerSearch, mttJobNumberSearch, selectedEngineer).ToList();

            IEnumerable<ServiceSheet> sortedServiceSheets;

            currentSortOrder = String.IsNullOrEmpty(currentSortOrder) ? ServiceSheetListVM.customerSortAsc : currentSortOrder;


            switch (currentSortOrder)
            {
                case ServiceSheetListVM.customerSortAsc:
                    sortedServiceSheets = serviceSheetsFound.OrderBy(s => s.Customer);
                    break;
                case ServiceSheetListVM.customerSortDesc:
                    sortedServiceSheets = serviceSheetsFound.OrderByDescending(s => s.Customer);
                    break;
                case ServiceSheetListVM.submissionSortAsc:
                    sortedServiceSheets = serviceSheetsFound.OrderBy(s => s.SubmissionNumber);
                    break;
                case ServiceSheetListVM.submissionSortDesc:
                    sortedServiceSheets = serviceSheetsFound.OrderByDescending(s => s.SubmissionNumber);
                    break;
                case ServiceSheetListVM.jobNumberSortAsc:
                    sortedServiceSheets = serviceSheetsFound.OrderBy(s => s.MttJobNumber);
                    break;
                case ServiceSheetListVM.jobNumberSortDesc:
                    sortedServiceSheets = serviceSheetsFound.OrderByDescending(s => s.MttJobNumber);
                    break;
                case ServiceSheetListVM.machineSortAsc:
                    sortedServiceSheets = serviceSheetsFound.OrderBy(m => m.MachineMakeModel);
                    break;
                case ServiceSheetListVM.machineSortDesc:
                    sortedServiceSheets = serviceSheetsFound.OrderByDescending(m => m.MachineMakeModel);
                    break;
                case ServiceSheetListVM.engineerSortAsc:
                    sortedServiceSheets = serviceSheetsFound.OrderBy(m => m.EngineerFullName);
                    break;
                case ServiceSheetListVM.engineerSortDesc:
                    sortedServiceSheets = serviceSheetsFound.OrderByDescending(m => m.EngineerFullName);
                    break;
                default:
                    sortedServiceSheets = serviceSheetsFound.OrderBy(s => s.Customer);
                    break;
            }

            vm.CurrentSortOrder = currentSortOrder;
            vm.ServiceSheets = sortedServiceSheets.ToPagedList(pageNumber, pageSize);

            return View(vm);
        }

        // GET: ServiceSheet
        public async Task<ActionResult> Index([Bind] int? page, int? submissionNumber, DateTime? sheetsFromDateSearch, DateTime? sheetsToDateSearch, string customerSearch,
                                            string mttJobNumberSearch, string selectedEngineer)
        {
            ServiceSheetIndexVM vm = new ServiceSheetIndexVM();

            //Populate the list of engineers
            List<SelectListItem> engineerSL = await CreateEngineerList();
            vm.Engineers = engineerSL;

            //Return the current search terms to include in the view
            vm.SubmissionNumber = submissionNumber;
            vm.SheetsFromDateSearch = sheetsFromDateSearch;
            vm.SheetsToDateSearch = sheetsToDateSearch;
            vm.CustomerSearch = customerSearch;
            vm.MttJobNumberSearch = mttJobNumberSearch;
            vm.SelectedEngineer = selectedEngineer;

            return View(vm);
        }

        public IQueryable<ServiceSheet> GetServiceSheets(int? submissionNumber, DateTime? sheetsFromDateSearch, DateTime? sheetsToDateSearch, string customerSearch,
                                            string mttJobNumberSearch, string selectedEngineer)
        {
            var result = db.ServiceSheets.AsQueryable();

            if (submissionNumber.HasValue)
            {
                result = result.Where(s => s.SubmissionNumber == submissionNumber);
            }
            if (sheetsFromDateSearch.HasValue)
            {
                result = result.Where(s => s.ServiceDays.Any(d => d.DtReport >= sheetsFromDateSearch));
            }
            if (sheetsToDateSearch.HasValue)
            {
                result = result.Where(s => s.ServiceDays.Any(d => d.DtReport <= sheetsToDateSearch));
            }
            if (!string.IsNullOrEmpty(customerSearch))
            {
                result = result.Where(s => s.Customer.Contains(customerSearch));
            }
            if (!string.IsNullOrEmpty(mttJobNumberSearch))
            {
                result = result.Where(s => s.MttJobNumber.Contains(mttJobNumberSearch));
            }
            if (!string.IsNullOrEmpty(selectedEngineer))
            {
                result = result.Where(s => s.Username.Equals(selectedEngineer));
            }
            return result;
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

            return engineerSL.OrderBy(m => m.Text).ToList();
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