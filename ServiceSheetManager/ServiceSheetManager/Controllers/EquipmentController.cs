using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using ServiceSheetManager.ViewModels.EquipmentVMs;
using System.Web.Mvc;
using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModelAssemblers;

namespace ServiceSheetManager.Controllers
{
    public class EquipmentController : Controller
    {
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        // GET: Equipment
        public async Task<ActionResult> Index([Bind] int? SelectedEquipmentTypeId)
        {
            //RT 30/1/19 - Removing the items marked as deleted from the index
            //var equipments = db.Equipments.Include(e => e.EquipmentKit)
            //                                .Include(e => e.EquipmentLocations)
            //                                .Include(e => e.EquipmentKit.Equipments.Select(loc => loc.EquipmentLocations))
            //                                .Include(e => e.EquipmentCalibrations);
            EquipmentType deletedType = await RetrieveDeletedType();
            var equipments = db.Equipments.Where(e => e.EquipmentTypeId != deletedType.Id && e.EquipmentKit.EquipmentTypeId != deletedType.Id).Include(e => e.EquipmentKit)
                                           .Include(e => e.EquipmentLocations)
                                           .Include(e => e.EquipmentKit.Equipments.Select(loc => loc.EquipmentLocations))
                                           .Include(e => e.EquipmentCalibrations);


            //Apply the filters
            if (SelectedEquipmentTypeId.HasValue && SelectedEquipmentTypeId.Value != -1)
            {
                equipments = equipments.Where(e => e.EquipmentTypeId == SelectedEquipmentTypeId.Value || e.EquipmentKit.EquipmentTypeId == SelectedEquipmentTypeId.Value);
            }

            //RT 30/1/19 - Removing the items marked as deleted from the selectlist
            //var equipmentTypes = await db.EquipmentTypes.ToListAsync();
            var equipmentTypes = await db.EquipmentTypes.Where(e => e.Id != deletedType.Id).ToListAsync();

            //Create the VMs
            EquipmentVMAssembler vMAssembler = new EquipmentVMAssembler();

            EquipmentIndexVM indexVM = await vMAssembler.CreateEquipmentIndex(equipments, equipmentTypes);

            return View(indexVM);
        }

        // GET: Equipment/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //RT 15/1/18 - Adding in equipment type
            //Equipment equipment = await db.Equipments.FindAsync(id);
            //RT 28/1/18 - Adding in the latest calibration record
            //Equipment equipment = await db.Equipments.Where(e => e.Id == id.Value).Include(e => e.EquipmentType).FirstOrDefaultAsync();
            Equipment equipment = await db.Equipments.Where(e => e.Id == id.Value).Include(e => e.EquipmentType).Include(e => e.EquipmentCalibrations).FirstOrDefaultAsync();

            if (equipment == null)
            {
                return HttpNotFound();
            }

            EquipmentVMAssembler vmAssembler = new EquipmentVMAssembler();

            DisplayEquipmentItemVM displayVM = vmAssembler.DisplayEquipmentVM(equipment);

            return View(displayVM);
        }

        // GET: Equipment/Create
        public async Task<ActionResult> Create()
        {
            //Need to load the equipment Types to the view
            EquipmentVMAssembler assembler = new EquipmentVMAssembler();

            SelectList equipmentTypesSL = await EquipmentTypeVMAssembler.GetAllTypes(db, null);

            CreateEquipmentItemVM equipmentVM = assembler.CreateEquipmentItem(equipmentTypesSL);

            return View(equipmentVM);
        }

        // POST: Equipment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEquipmentItemVM equipmentVM)
        {
            if (String.IsNullOrEmpty(equipmentVM.Barcode))
            {
                ModelState.AddModelError("Barcode", "Barcode is required");
            }

            if (ModelState.IsValid)
            {
                var viewModelBuilder = new EquipmentVMAssembler();

                //Check that the barcode is unique
                bool uniqueBarcode = viewModelBuilder.BarcodeIsUnique(equipmentVM.Barcode, null, true, db);
                if (!uniqueBarcode)
                {
                    ModelState.AddModelError("", "Barcode already exists");
                    await equipmentVM.RepopulateSelectLists(db);

                    return View(equipmentVM);
                }

                //Create the equipment db item
                Equipment itemToSave = viewModelBuilder.Map(equipmentVM);
                db.Equipments.Add(itemToSave);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            await equipmentVM.RepopulateSelectLists(db);
            return View(equipmentVM);
        }

        // GET: Equipment/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Adding in equipment type
            //Equipment equipment = await db.Equipments.FindAsync(id);
            Equipment equipment = await db.Equipments.Where(e => e.Id == id.Value).Include(e => e.EquipmentType).FirstOrDefaultAsync();

            if (equipment == null)
            {
                return HttpNotFound();
            }

            EquipmentVMAssembler vmAssembler = new EquipmentVMAssembler();
            
            SelectList equipmentTypesSL = await EquipmentTypeVMAssembler.GetAllTypes(db, equipment.EquipmentTypeId);

            EditEquipmentItemVM editVM = vmAssembler.EditEquipmentVM(equipment, equipmentTypesSL);
            //ViewBag.EquipmentKitId = new SelectList(db.EquipmentKits, "Id", "Barcode", equipment.EquipmentKitId);
            return View(editVM);
        }

        // POST: Equipment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditEquipmentItemVM equipmentVM)
        {
            if (ModelState.IsValid)
            {
                var vmBuilder = new EquipmentVMAssembler();

                //Check that the barcode is unique
                bool uniqueBarcode = vmBuilder.BarcodeIsUnique(equipmentVM.Barcode, equipmentVM.Id, true, db);
                if (!uniqueBarcode)
                {
                    ModelState.AddModelError("", "Barcode already exists");
                    await equipmentVM.RepopulateSelectLists(db);
                    return View(equipmentVM);
                }

                //Load the original
                Equipment equipmentToUpdate = await db.Equipments.Where(e => e.Id == equipmentVM.Id).FirstOrDefaultAsync();
                equipmentToUpdate = vmBuilder.Map(equipmentVM, equipmentToUpdate);
                db.Entry(equipmentToUpdate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            await equipmentVM.RepopulateSelectLists(db);
            return View(equipmentVM);
        }

        // GET: Equipment/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Equipment equipment = await db.Equipments.Where(e => e.Id == id.Value).Include(e => e.EquipmentType).Include(e => e.EquipmentCalibrations).FirstOrDefaultAsync();

            if (equipment == null)
            {
                return HttpNotFound();
            }

            EquipmentVMAssembler vmAssembler = new EquipmentVMAssembler();

            DisplayEquipmentItemVM displayVM = vmAssembler.DisplayEquipmentVM(equipment);

           
            return View(displayVM);
        }

        //POST: Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //RT 30/1/19 - On delete, just set the type to the deleted type, to keep the records
            EquipmentType deletedType = await RetrieveDeletedType();

            Equipment equipment = await db.Equipments.FindAsync(id);
            equipment.EquipmentType = deletedType;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //RT 30/01/19 - Method to return the deleted type from the database.  This is used to mark equipment as deleted
        private async Task<EquipmentType> RetrieveDeletedType()
        {
            EquipmentType deletedType = await db.EquipmentTypes.Where(e => e.Description.Equals("Deleted")).FirstAsync();

            if (deletedType == null)
            {
                throw new Exception("Deleted type not found");
            }
            return deletedType;
        }

        //RT 30/1/19 - Adding functionality to mark equipment as away for calibration
        public async Task<ActionResult> CalibrateEquipment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Equipment equipment = await db.Equipments.Where(e => e.Id == id.Value).Include(e => e.EquipmentType).Include(e => e.EquipmentCalibrations).FirstOrDefaultAsync();

            if (equipment == null)
            {
                return HttpNotFound();
            }

            EquipmentVMAssembler vmAssembler = new EquipmentVMAssembler();

            DisplayEquipmentItemVM displayVM = vmAssembler.DisplayEquipmentVM(equipment);
            return View(displayVM);
        }

        [HttpPost, ActionName("CalibrateEquipment")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CalibrateEquipmentConfirmed(int id)
        {
            Equipment equipment = await db.Equipments.FindAsync(id);

            EquipmentLocation calLocation = new EquipmentLocation();
            calLocation.CanvasSubmissionNumber = -1;
            calLocation.DtScanned = DateTime.Now;
            calLocation.Equipment = equipment;
            calLocation.LocationCode = 3;
            calLocation.ScannedUserFirstName = "Away for Calibration";
            calLocation.ScannedUserName = "";
            calLocation.ScannedUserSurname = "";

            db.EquipmentLocations.Add(calLocation);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
