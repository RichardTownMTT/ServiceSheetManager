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
            var allEquipment = db.Equipments.Include(e => e.EquipmentKit).Include(e => e.EquipmentKit.Equipments.Select(loc => loc.EquipmentLocations)).
                                            Include(e => e.EquipmentCalibrations).Include(e => e.EquipmentLocations);

            EquipmentCalibrationDueReportVM vm = await EquipmentReportsVMAssembler.GenerateReportVM(allEquipment);

            return View(vm);
        }

        //public async Task<ActionResult> EquipmentLocationHistory(EquipmentLocationHistoryVM returnedVM)
        //{
        //    EquipmentLocationHistoryVM vm = new EquipmentLocationHistoryVM(); ;

        //    if (returnedVM.SelectedEquipmentItemBarcode == null)
        //    {
        //        var allEquipmentAndKit = await db.Equipments.Include(e => e.EquipmentKit).ToListAsync();

        //        vm = EquipmentLocationHistoryReportVMAssembler.LoadEquipmentOnly(allEquipmentAndKit);
        //    }
        //    else
        //    {
        //        var allEquipmentKitAndLocations = await db.Equipments.Include(e => e.EquipmentKit).Include(e => e.EquipmentLocations).ToListAsync();


        //        vm = EquipmentLocationHistoryReportVMAssembler.LoadEquipmentAndHistory(allEquipmentKitAndLocations, returnedVM.SelectedEquipmentItemBarcode);
        //    }

        //    return View(vm);
        //}
    }
}