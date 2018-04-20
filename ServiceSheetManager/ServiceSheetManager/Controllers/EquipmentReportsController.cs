using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Web.Mvc;
using ServiceSheetManager.ViewModelAssemblers;
using ServiceSheetManager.ViewModels.EquipmentVMs;

namespace ServiceSheetManager.Controllers
{
    public class EquipmentReportsController : Controller
    {

        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        // GET: EquipmentReports
        public async Task<ActionResult> CalibrationDueOrder()
        {
            var allEquipment = db.Equipments.Include(e => e.EquipmentKit).Include(e => e.EquipmentKit).Include(e => e.EquipmentCalibrations);

            EquipmentCalibrationDueReportVM vm = await EquipmentReportsVMAssembler.GenerateReportVM(allEquipment);

            return View(vm);
        }
    }
}