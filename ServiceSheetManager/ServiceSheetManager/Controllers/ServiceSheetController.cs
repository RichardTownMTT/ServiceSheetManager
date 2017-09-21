using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using PagedList;
using ServiceSheetManager.Helpers;
using System.IO;
using PdfSharp.Pdf;

namespace ServiceSheetManager.Controllers
{
    public class ServiceSheetController : Controller
    {
        private const int pageSize = 10;
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        //Get
        public async Task<ActionResult> Edit(int? submissionNumber)
        {
            if (!submissionNumber.HasValue)
            {
                return View(model: null);
            }
            else
            {
                ServiceSheet serviceSheetEdit = await db.ServiceSheets.Where(s => s.SubmissionNumber == submissionNumber.Value).Include(s => s.ServiceDays).FirstOrDefaultAsync();

                //Create the view model from the Service sheet
                ServiceSheetVM vm = new ServiceSheetVM(serviceSheetEdit);
                return View(vm);
            }
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ServiceSheetVM submittedSheet)
        {
            //Validate the service sheet
            if (!ModelState.IsValid)
            {
                return View(submittedSheet);
            }

            ServiceSheet updateSheet = await db.ServiceSheets.Where(s => s.Id == submittedSheet.Id).Include(s => s.ServiceDays).FirstOrDefaultAsync();

            submittedSheet.UpdateModel(updateSheet);

            //Update the totals, etc.
            ServiceSheetHelpers.UpdateServiceSheetTotals(updateSheet); 

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Error saving changes to service sheet: " + submittedSheet.Id);
            }

            return RedirectToAction("Index");
        }

        public ActionResult GenerateServiceReport(int? SubmissionNumber, bool includeImage1, bool includeImage2, bool includeImage3, bool includeImage4, bool includeImage5, bool includeCustomerSignature)
        {
            if (!SubmissionNumber.HasValue)
            {
                return RedirectToAction("Error");
            }

            ServiceSheet sheet = db.ServiceSheets.Where(s => s.SubmissionNumber == SubmissionNumber.Value).Include(s => s.ServiceDays).FirstOrDefault();

            if (sheet == null)
            {
                return RedirectToAction("Error");
            }

            using (MemoryStream stream = new MemoryStream())
            {
                PdfServiceSheetCreator creator = new PdfServiceSheetCreator();
                PdfDocument pdfDoc = creator.CreatePdfSheetForSubmission(sheet, includeImage1, includeImage2, includeImage3, includeImage4, includeImage5, includeCustomerSignature);

                pdfDoc.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }

        public ActionResult ListReports([Bind] int? page, int? submissionNumber, DateTime? sheetsFromDateSearch, DateTime? sheetsToDateSearch, string customerSearch,
                                            string mttJobNumberSearch, string selectedEngineer, string currentSortOrder)
        {
            int pageNumber = (page ?? 1);
            ServiceSheetListVM vm = new ServiceSheetListVM();

            List<ServiceSheet> serviceSheetsFound = GetServiceSheets(submissionNumber, sheetsFromDateSearch, sheetsToDateSearch, customerSearch, mttJobNumberSearch, selectedEngineer).ToList();
            List<ServiceSheetVM> serviceSheetsFoundVMs = new List<ServiceSheetVM>();
            foreach (var sheet in serviceSheetsFound)
            {
                ServiceSheetVM sheetVM = new ServiceSheetVM(sheet);
                serviceSheetsFoundVMs.Add(sheetVM);
            }

            IEnumerable<ServiceSheetVM> sortedServiceSheets;

            currentSortOrder = String.IsNullOrEmpty(currentSortOrder) ? ServiceSheetListVM.customerSortAsc : currentSortOrder;


            switch (currentSortOrder)
            {
                case ServiceSheetListVM.customerSortAsc:
                    sortedServiceSheets = serviceSheetsFoundVMs.OrderBy(s => s.Customer);
                    break;
                case ServiceSheetListVM.customerSortDesc:
                    sortedServiceSheets = serviceSheetsFoundVMs.OrderByDescending(s => s.Customer);
                    break;
                case ServiceSheetListVM.submissionSortAsc:
                    sortedServiceSheets = serviceSheetsFoundVMs.OrderBy(s => s.SubmissionNumber);
                    break;
                case ServiceSheetListVM.submissionSortDesc:
                    sortedServiceSheets = serviceSheetsFoundVMs.OrderByDescending(s => s.SubmissionNumber);
                    break;
                case ServiceSheetListVM.jobNumberSortAsc:
                    sortedServiceSheets = serviceSheetsFoundVMs.OrderBy(s => s.MttJobNumber);
                    break;
                case ServiceSheetListVM.jobNumberSortDesc:
                    sortedServiceSheets = serviceSheetsFoundVMs.OrderByDescending(s => s.MttJobNumber);
                    break;
                case ServiceSheetListVM.machineSortAsc:
                    sortedServiceSheets = serviceSheetsFoundVMs.OrderBy(m => m.MachineMakeModel);
                    break;
                case ServiceSheetListVM.machineSortDesc:
                    sortedServiceSheets = serviceSheetsFoundVMs.OrderByDescending(m => m.MachineMakeModel);
                    break;
                case ServiceSheetListVM.engineerSortAsc:
                    sortedServiceSheets = serviceSheetsFoundVMs.OrderBy(m => m.EngineerFullName);
                    break;
                case ServiceSheetListVM.engineerSortDesc:
                    sortedServiceSheets = serviceSheetsFoundVMs.OrderByDescending(m => m.EngineerFullName);
                    break;
                default:
                    sortedServiceSheets = serviceSheetsFoundVMs.OrderBy(s => s.Customer);
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

            List<SelectListItem> engineerSL = new List<SelectListItem>
            {
                //Add blank engineer
                new SelectListItem() { Text = "", Value = "" }
            };
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

            ServiceSheetVM sheetVM = new ServiceSheetVM(sheet);
            sheetVM.LoadCanvasImages();

            return View(sheetVM);
        }
    }
}