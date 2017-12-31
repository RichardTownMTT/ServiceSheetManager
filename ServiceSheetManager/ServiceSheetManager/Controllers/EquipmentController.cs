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
using ServiceSheetManager.ViewModels.EquipmentVMs.EquipmentVMBuilders;

namespace ServiceSheetManager.Controllers
{
    public class EquipmentController : Controller
    {
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        // GET: Equipment
        public async Task<ActionResult> Index()
        {
            var equipments = db.Equipments.Include(e => e.EquipmentKit);
            //Create the VMs
            EquipmentListVM displayVm = new EquipmentListVM();
            await equipments.ToListAsync();

            foreach (var equipmentItem in equipments)
            {
                DisplayEquipmentListItemVM item = new DisplayEquipmentListItemVM(equipmentItem);
                displayVm.AllItems.Add(item);
            }
            return View(displayVm);
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

            DisplayEquipmentItemVM displayVM = new DisplayEquipmentItemVM(equipment);

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
            if (ModelState.IsValid)
            {
                var viewModelBuilder = new EquipmentViewModelBuilder();

                //Check that the barcode is unique
                bool uniqueBarcode = BarcodeIsUnique(equipmentVM.Barcode, null, true);
                if (!uniqueBarcode)
                {
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
            EditEquipmentItemVM editVM = new EditEquipmentItemVM(equipment);
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
                var vmBuilder = new EquipmentViewModelBuilder();

                //Check that the barcode is unique
                bool uniqueBarcode = BarcodeIsUnique(equipmentVM.Barcode, equipmentVM.Id, true);
                if (!uniqueBarcode)
                {
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

        // GET: Equipment/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
            return View(equipment);
        }

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

        private bool BarcodeIsUnique(string barcode, int? id, bool equipmentMode)
        {
            bool barcodeUnique = false;
            int idCheck;
            if(!id.HasValue)
            {
                idCheck = -1;
            }
            else
            {
                idCheck = id.Value;
            }

            //Find all equipment with barcode
            List<Equipment> allEquipmentWithBarcode = db.Equipments.Where(e => e.Barcode.Equals(barcode)).ToList();
            if (allEquipmentWithBarcode.Count == 0)
            {
                barcodeUnique = true;
            }
            else if (allEquipmentWithBarcode.Count == 1)
            {
                Equipment match = allEquipmentWithBarcode.First();
                if (match.Id == idCheck && equipmentMode == true)
                {
                    barcodeUnique = true;
                }
                else
                {
                    barcodeUnique = false;
                }
            }
            else
            {
                throw new Exception("Multiple items with barcode!");
            }

            List<EquipmentKit> allKitWithBarcode = db.EquipmentKits.Where(e => e.Barcode.Equals(barcode)).ToList();
            if (allKitWithBarcode.Count == 0)
            {
                barcodeUnique = true;
            }
            else if (allKitWithBarcode.Count == 1)
            {
                EquipmentKit match = allKitWithBarcode.First();
                if (match.Id == idCheck && equipmentMode == false)
                {
                    barcodeUnique = true;
                }
                else
                {
                    barcodeUnique = false;
                }
            }
            else
            {
                throw new Exception("Multiple items with barcode!");
            }

            return barcodeUnique;
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
