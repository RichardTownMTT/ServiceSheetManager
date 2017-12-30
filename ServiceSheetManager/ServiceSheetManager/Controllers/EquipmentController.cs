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
                //Check that the barcode is unique
                bool uniqueBarcode = BarcodeIsUnique(equipmentVM.Barcode);
                if (!uniqueBarcode)
                {
                    return View(equipmentVM);
                }

                //Create the equipment db item
                Equipment itemToSave = CreateEquipmentFromVM(equipmentVM);
                db.Equipments.Add(itemToSave);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(equipmentVM);
        }

        private bool BarcodeIsUnique(string barcode)
        {
            bool barcodeUnique = false;
            var barcodeFoundEquipment = db.Equipments.FirstOrDefault(e => e.Barcode.Equals(barcode)).Barcode.ToString();
            if (barcodeFoundEquipment == null)
            {
                //Try the equipment kit
                var kitBarcodeFound = db.EquipmentKits.FirstOrDefault(e => e.Barcode.Equals(barcode)).Barcode.ToString();
                if (kitBarcodeFound == null)
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
                barcodeUnique = false;
            }

            return barcodeUnique;
        }

        private Equipment CreateEquipmentFromVM(CreateEquipmentItemVM equipmentVM)
        {
            Equipment retval = new Equipment
            {
                Barcode = equipmentVM.Barcode,
                CalibrationPeriodDays = equipmentVM.CalibrationPeriodDays,
                Description = equipmentVM.Description,
                SerialNumber = equipmentVM.SerialNumber
            };
            return retval;
        }
        private Equipment CreateEquipmentFromVM(EditEquipmentItemVM equipmentVM)
        {
            Equipment retval = new Equipment
            {
                Id = equipmentVM.Id,
                Barcode = equipmentVM.Barcode,
                CalibrationPeriodDays = equipmentVM.CalibrationPeriodDays,
                Description = equipmentVM.Description,
                SerialNumber = equipmentVM.SerialNumber
            };
            return retval;
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
                //Check that the barcode is unique
                bool uniqueBarcode = BarcodeIsUnique(equipmentVM.Barcode);
                if (!uniqueBarcode)
                {
                    return View(equipmentVM);
                }
                Equipment saveEquipment = CreateEquipmentFromVM(equipmentVM);
                db.Entry(saveEquipment).State = EntityState.Modified;
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
