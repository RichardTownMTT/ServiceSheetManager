using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModelAssemblers;
using ServiceSheetManager.ViewModels.EquipmentVMs;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ServiceSheetManager.Controllers
{
    public class EquipmentKitsController : Controller
    {
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        // GET: EquipmentKits
        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.EquipmentKits.ToListAsync());
        //}

        // GET: EquipmentKits/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //RT 16/1/17 - Changing to include the equipment type
            //EquipmentKit equipmentKit = await db.EquipmentKits.Where(k => k.Id == id.Value).Include(e => e.Equipments).FirstOrDefaultAsync();
            //EquipmentKit equipmentKit = await db.EquipmentKits.Where(k => k.Id == id.Value).Include(e => e.Equipments).Include(k => k.EquipmentType).FirstOrDefaultAsync();
            //RT 28/1/19 - Adding in the calibration record
            EquipmentKit equipmentKit = await db.EquipmentKits.Where(k => k.Id == id.Value).Include(e => e.Equipments).Include(k => k.EquipmentType)
                                                .Include(e => e.Equipments.Select(c => c.EquipmentCalibrations)).FirstOrDefaultAsync();

            if (equipmentKit == null)
            {
                return HttpNotFound();
            }

            EquipmentKitVMAssembler vmKitAssesmbler = new EquipmentKitVMAssembler();
            DisplayEquipmentKitVM displayVM = vmKitAssesmbler.Display(equipmentKit);

            return View(displayVM);
        }

        // GET: EquipmentKits/Create
        public async Task<ActionResult> Create()
        {
            EquipmentKitVMAssembler assembler = new EquipmentKitVMAssembler();

            //Load the available equipment types
            SelectList equipmentTypes = await EquipmentTypeVMAssembler.GetAllTypes(db, null);

            CreateEquipmentKitVM vm = assembler.Create(equipmentTypes);

            vm.EquipmentItems.Add(new CreateEquipmentItemVM(null));
            vm.EquipmentItems.Add(new CreateEquipmentItemVM(null));
            vm.EquipmentItems.Add(new CreateEquipmentItemVM(null));
            vm.EquipmentItems.Add(new CreateEquipmentItemVM(null));
            vm.EquipmentItems.Add(new CreateEquipmentItemVM(null));
            vm.EquipmentItems.Add(new CreateEquipmentItemVM(null));
            return View(vm);
        }

        // POST: EquipmentKits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateEquipmentKitVM equipmentVM)
        {
            
            if (ModelState.IsValid)
            {
                var equipmentViewModelAssembler = new EquipmentVMAssembler();

                //Check that the barcode is unique
                bool uniqueBarcode = equipmentViewModelAssembler.BarcodeIsUnique(equipmentVM.Barcode, null, false, db);
                if (!uniqueBarcode)
                {
                    ModelState.AddModelError("", "Barcode already exists");

                    await equipmentVM.RepopulateSelectLists(db, int.Parse(equipmentVM.SelectedEquipmentTypeId));

                    return View(equipmentVM);
                }

                var kitViewModelAssembler = new EquipmentKitVMAssembler();

                //Map the equipment and kit from the view model.  Each is handled by a separate assessmbler
                EquipmentKit kitToSave = kitViewModelAssembler.Map(equipmentVM);

                db.EquipmentKits.Add(kitToSave);

                List<CreateEquipmentItemVM> equipmentItemVMs = equipmentVM.EquipmentItems;
                List<Equipment> equipmentToSave = equipmentViewModelAssembler.Map(equipmentItemVMs);

                foreach (var equipmentItem in equipmentToSave)
                {
                    equipmentItem.EquipmentKit = kitToSave;
                    db.Equipments.Add(equipmentItem);
                }
                
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Equipment");
            }

            //If not valid, we have to re-populate the select list
            await equipmentVM.RepopulateSelectLists(db, int.Parse(equipmentVM.SelectedEquipmentTypeId));

            return View(equipmentVM);
        }

        public ActionResult AddNewEquipment()
        {
            var equipmentVM = new CreateEquipmentItemVM(null);

            return PartialView("~/Views/Shared/EditorTemplates/CreateEquipmentItemVM.cshtml", equipmentVM);
        }

        // GET: EquipmentKits/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    EquipmentKit equipmentKit = await db.EquipmentKits.FindAsync(id);
        //    if (equipmentKit == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(equipmentKit);
        //}

        //// POST: EquipmentKits/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,Barcode,Description")] EquipmentKit equipmentKit)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(equipmentKit).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(equipmentKit);
        //}

        // GET: EquipmentKits/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    EquipmentKit equipmentKit = await db.EquipmentKits.FindAsync(id);
        //    if (equipmentKit == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(equipmentKit);
        //}

        //// POST: EquipmentKits/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    EquipmentKit equipmentKit = await db.EquipmentKits.FindAsync(id);
        //    db.EquipmentKits.Remove(equipmentKit);
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
