using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceSheetManager.ViewModels.EquipmentVMs;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ServiceSheetManager.ViewModelAssemblers
{
    public class EquipmentVMAssembler
    {
        public Equipment Map(CreateEquipmentItemVM equipmentVM)
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

        public Equipment Map(EditEquipmentItemVM equipmentVM, Equipment originalEquipment)
        {
            originalEquipment.Barcode = equipmentVM.Barcode;
            originalEquipment.CalibrationPeriodDays = equipmentVM.CalibrationPeriodDays;
            originalEquipment.Description = equipmentVM.Description;
            originalEquipment.SerialNumber = equipmentVM.SerialNumber;

            return originalEquipment;
        }

        public async Task<EquipmentIndexVM> CreateEquipmentIndex(IQueryable<Equipment> equipments)
        {
            //Go through all the equipment that is in a kit first
            List<EquipmentKit> equipmentInKit = await equipments.Where(e => e.EquipmentKitId.HasValue).Select(k => k.EquipmentKit).Distinct().ToListAsync();

            EquipmentIndexVM retval = new EquipmentIndexVM();
            foreach (var kitEquipment in equipmentInKit)
            {
                EquipmentIndexKitItemVM kitItem = new EquipmentIndexKitItemVM(kitEquipment);
                retval.AllKits.Add(kitItem);
            }

            List<Equipment> equipmentOnly = await equipments.Where(e => e.EquipmentKitId.HasValue == false).ToListAsync();
            foreach (var equipmentNotInKit in equipmentOnly)
            {
                EquipmentIndexEquipmentItemVM equipmentItem = new EquipmentIndexEquipmentItemVM(equipmentNotInKit);
                retval.AllEquipmentNotInKitItems.Add(equipmentItem);
            }

            return retval;
        }

        public DisplayEquipmentItemVM DisplayEquipmentVM(Equipment equipment)
        {
            DisplayEquipmentItemVM retval = new DisplayEquipmentItemVM(equipment);

            return retval;
        }

        public bool BarcodeIsUnique(string barcode, int? id, bool equipmentMode, ServiceSheetsEntities db)
        {
            bool barcodeUnique = false;
            int idCheck;
            if (!id.HasValue)
            {
                idCheck = -1;
            }
            else
            {
                idCheck = id.Value;
            }

            bool equipmentBarcodeUnique = CheckEquipmentBarcodeUnique(barcode, idCheck, equipmentMode, db);

            bool equipmentKitBarcodeUnique = CheckEquipmentKitBarcodeUnique(barcode, idCheck, equipmentMode, db);

            if (equipmentBarcodeUnique && equipmentKitBarcodeUnique)
            {
                barcodeUnique = true;
            }

            return barcodeUnique;
        }

        public List<Equipment> Map(List<CreateEquipmentItemVM> equipmentItemVMs)
        {
            List<Equipment> retval = new List<Equipment>();

            foreach (var item in equipmentItemVMs)
            {
                Equipment itemCreated = Map(item);
                retval.Add(itemCreated);
            }

            return retval;
        }

        private bool CheckEquipmentKitBarcodeUnique(string barcode, int idCheck, bool equipmentMode, ServiceSheetsEntities db)
        {
            bool barcodeUnique = false;
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

        public EditEquipmentItemVM EditEquipmentVM(Equipment equipment)
        {
            EditEquipmentItemVM vm = new EditEquipmentItemVM(equipment);
            return vm;
        }

        private bool CheckEquipmentBarcodeUnique(string barcode, int idCheck, bool equipmentMode, ServiceSheetsEntities db)
        {
            bool barcodeUnique = false;
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

            return barcodeUnique;
        }
    }
}