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
            var equipments = db.Equipments.Include(e => e.EquipmentKit)
                                            .Include(e => e.EquipmentLocations)
                                            .Include(e => e.EquipmentKit.Equipments.Select(loc => loc.EquipmentLocations));

            //Apply the filters
            if (SelectedEquipmentTypeId.HasValue && SelectedEquipmentTypeId.Value != -1)
            {
                equipments = equipments.Where(e => e.EquipmentTypeId == SelectedEquipmentTypeId.Value || e.EquipmentKit.EquipmentTypeId == SelectedEquipmentTypeId.Value);
            }

            var equipmentTypes = await db.EquipmentTypes.ToListAsync();
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
            Equipment equipment = await db.Equipments.Where(e => e.Id == id.Value).Include(e => e.EquipmentType).FirstOrDefaultAsync();

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

        //// GET: Equipment/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Equipment equipment = await db.Equipments.FindAsync(id);
        //    if (equipment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(equipment);
        //}

        // POST: Equipment/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Equipment equipment = await db.Equipments.FindAsync(id);
        //    db.Equipments.Remove(equipment);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        

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
