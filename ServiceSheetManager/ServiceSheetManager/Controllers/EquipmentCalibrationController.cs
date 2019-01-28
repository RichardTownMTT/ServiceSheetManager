using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModelAssemblers;
using ServiceSheetManager.ViewModels.EquipmentVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Web.Mvc;

namespace ServiceSheetManager.Controllers
{
    public class EquipmentCalibrationController : Controller
    {
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        // GET: EquipmentCalibration/Create
        public async Task<ActionResult> Create(CreateEquipmentCalibrationVM equipmentCalVMReturned)
        {

            //Need to load the equipment Types to the view
            EquipmentCalibrationVMAssembler assembler = new EquipmentCalibrationVMAssembler();

            List<Equipment> allEquipmentAndKits = await db.Equipments.Where(e => e.CalibrationPeriodYears != null).Include(e => e.EquipmentKit).ToListAsync();

            CreateEquipmentCalibrationVM equipmentCalVM = assembler.CreateEquipmentSearchView(allEquipmentAndKits, equipmentCalVMReturned);

            return View(equipmentCalVM);
        }

        // POST: Equipment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSave(CreateEquipmentCalibrationVM equipmentCalVMReturned)
        {
            EquipmentCalibration cal = new EquipmentCalibration();
            cal.EquipmentId = equipmentCalVMReturned.IdSelected;
            cal.CertNumber = equipmentCalVMReturned.CertificateNo;
            cal.DtCalibrated = equipmentCalVMReturned.DtPassedCal;
            cal.Passed = equipmentCalVMReturned.PassedCalibration;

            if (cal.DtCalibrated == new DateTime())
            {
                ModelState.AddModelError("", "Date is required");
            }

            
            if (ModelState.IsValid)
            {
                db.EquipmentCalibrations.Add(cal);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Equipment");
            }

            equipmentCalVMReturned.EquipmentLoaded = true;
            equipmentCalVMReturned.SelectedEquipmentItem = equipmentCalVMReturned.IdSelected.ToString();
            return RedirectToAction("Create", equipmentCalVMReturned);

        }
    }
}