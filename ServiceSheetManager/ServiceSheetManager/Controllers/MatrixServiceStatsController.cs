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
using ServiceSheetManager.Models.NonDbModels;

namespace ServiceSheetManager.Controllers
{
    public class MatrixServiceStatsController : Controller
    {
        private ServiceSheetsEntities db = new ServiceSheetsEntities();
        // GET: MatrixServiceStats
        public async Task<ActionResult> Index()
        {
            MatrixServiceStatsVM statsVM = new MatrixServiceStatsVM();
            List<JobNumberItem> matrixJobNumbers = new List<JobNumberItem>();

            var jobNumbers = await db.ServiceSheets.Where(m => m.MttJobNumber.Contains("MAT/4537")).GroupBy(m => m.MttJobNumber).ToListAsync();

            foreach (var item in jobNumbers)
            {
                matrixJobNumbers.Add(new JobNumberItem { MttJobNumber = item.Key });
            }

            statsVM.AllJobNumbers = matrixJobNumbers;

            //Load the Service overview table
            var serviceOverview = await db.MatrixServiceOverviews.FirstOrDefaultAsync();

            if (serviceOverview != null)
            {
                statsVM.ServiceOverview = serviceOverview;
            }
            else
            {
                throw new Exception("Matrix stats missing.");
            }

            //statsVM.ServiceOverview = serviceOverview ?? throw new Exception("Matrix stats missing.");

            return View(statsVM);
        }

        
    }
}
