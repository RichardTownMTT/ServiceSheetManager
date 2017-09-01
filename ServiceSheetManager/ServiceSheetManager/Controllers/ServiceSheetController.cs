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
        public async Task<ActionResult> Index()
        {
            ServiceSheetIndexVM vm = new ServiceSheetIndexVM();

            var serviceSheets = await db.ServiceSheets.OrderByDescending(s => s.SubmissionNumber).Take(5).ToListAsync();

            vm.ServiceSheets = serviceSheets;

            return View(vm);
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