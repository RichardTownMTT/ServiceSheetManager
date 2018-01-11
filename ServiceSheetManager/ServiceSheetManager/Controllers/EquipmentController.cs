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
        public async Task<ActionResult> Index()
        { 
            var equipments = db.Equipments.Include(e => e.EquipmentKit)
                                            .Include(e => e.EquipmentLocations)
                                            .Include(e => e.EquipmentKit.Equipments.Select(loc => loc.EquipmentLocations));
            //Create the VMs
            EquipmentVMAssembler vMAssembler = new EquipmentVMAssembler();

            EquipmentIndexVM indexVM = await vMAssembler.CreateEquipmentIndex(equipments);

            return View(indexVM);
        }

        // GET: Equipment/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = await db.Equipments.FindAsync(id);
            
            if (equipment == null)
            {
                return HttpNotFound();
            }

            EquipmentVMAssembler vmAssembler = new EquipmentVMAssembler();

            DisplayEquipmentItemVM displayVM = vmAssembler.DisplayEquipmentVM(equipment);

            return View(displayVM);
        }

        // GET: Equipment/Create
        public ActionResult Create()
        {
            return View();
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
                    return View(equipmentVM);
                }

                //Create the equipment db item
                Equipment itemToSave = viewModelBuilder.Map(equipmentVM);
                db.Equipments.Add(itemToSave);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(equipmentVM);
        }

        // GET: Equipment/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = await db.Equipments.FindAsync(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }

            EquipmentVMAssembler vmAssembler = new EquipmentVMAssembler();

            EditEquipmentItemVM editVM = vmAssembler.EditEquipmentVM(equipment);
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
                    return View(equipmentVM);
                }

                //Load the original
                Equipment equipmentToUpdate = await db.Equipments.Where(e => e.Id == equipmentVM.Id).FirstOrDefaultAsync();
                equipmentToUpdate = vmBuilder.Map(equipmentVM, equipmentToUpdate);
                db.Entry(equipmentToUpdate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
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
