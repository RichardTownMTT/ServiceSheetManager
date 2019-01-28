using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceSheetManager.ViewModels.EquipmentVMs;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ServiceSheetManager.ViewModelAssemblers
{
    public class EquipmentVMAssembler
    {
        public Equipment Map(CreateEquipmentItemVM equipmentVM)
        {
            Equipment retval = new Equipment
            {
                Barcode = equipmentVM.Barcode,
                CalibrationPeriodYears = equipmentVM.CalibrationPeriodYears,
                Description = equipmentVM.Description,
                SerialNumber = equipmentVM.SerialNumber
            };

            //If part of a kit, then we won't set the equipment type
            if (!string.IsNullOrEmpty(equipmentVM.EquipmentTypeSelected))
            {
                retval.EquipmentTypeId = int.Parse(equipmentVM.EquipmentTypeSelected);
            }
                
            return retval;
        }

        public Equipment Map(EditEquipmentItemVM equipmentVM, Equipment originalEquipment)
        {
            originalEquipment.Barcode = equipmentVM.Barcode;
            originalEquipment.CalibrationPeriodYears = equipmentVM.CalibrationPeriodYears;
            originalEquipment.Description = equipmentVM.Description;
            originalEquipment.SerialNumber = equipmentVM.SerialNumber;
            originalEquipment.EquipmentTypeId = int.Parse(equipmentVM.EquipmentTypeSelected);

            return originalEquipment;
        }

        public async Task<EquipmentIndexVM> CreateEquipmentIndex(IQueryable<Equipment> equipments, List<EquipmentType> allEquipmentTypes)
        {
            //Go through all the equipment that is in a kit first
            //RT 15/1/18 - Adding in location
            //List<EquipmentKit> equipmentInKit = await equipments.Where(e => e.EquipmentKitId.HasValue).Select(k => k.EquipmentKit).Distinct().ToListAsync();
            //RT 17/1/18 - Adding in type
            //List<EquipmentKit> equipmentInKit = await equipments.Where(e => e.EquipmentKitId.HasValue).Select(k => k.EquipmentKit).Distinct().Include(k => k.EquipmentLocations).ToListAsync();
            List<EquipmentKit> equipmentInKit = await equipments.Where(e => e.EquipmentKitId.HasValue).Select(k => k.EquipmentKit).Distinct()
                                                    .Include(k => k.EquipmentLocations).Include(e => e.EquipmentType)
                                                    .Include(e => e.Equipments.Select(c => c.EquipmentCalibrations)).ToListAsync();



            EquipmentIndexVM retval = new EquipmentIndexVM();
            foreach (var kitEquipment in equipmentInKit)
            {
                EquipmentIndexKitItemVM kitItem = new EquipmentIndexKitItemVM(kitEquipment);
                retval.AllKits.Add(kitItem);
            }

            //RT 15/1/18 - Adding in location
            //List<Equipment> equipmentOnly = await equipments.Where(e => e.EquipmentKitId.HasValue == false).ToListAsync();
            //Adding in type
            //List<Equipment> equipmentOnly = await equipments.Where(e => e.EquipmentKitId.HasValue == false).Include(e => e.EquipmentLocations).ToListAsync();
            List<Equipment> equipmentOnly = await equipments.Where(e => e.EquipmentKitId.HasValue == false).Include(e => e.EquipmentLocations).Include(e => e.EquipmentType)
                                                    .Include(e => e.EquipmentCalibrations).ToListAsync();

            foreach (var equipmentNotInKit in equipmentOnly)
            {
                EquipmentIndexEquipmentItemVM equipmentItem = new EquipmentIndexEquipmentItemVM(equipmentNotInKit);
                retval.AllEquipmentNotInKitItems.Add(equipmentItem);
            }

            retval.AllEquipmentNotInKitItems = retval.AllEquipmentNotInKitItems.OrderBy(e => e.Barcode).ToList();
            retval.AllKits = retval.AllKits.OrderBy(e => e.Barcode).ToList();

            //Set the filter lists
            retval.EquipmentTypes = EquipmentTypeVMAssembler.GetFilterList(allEquipmentTypes);

            return retval;
        }

        public CreateEquipmentItemVM CreateEquipmentItem(SelectList equipmentTypesSL)
        {
            CreateEquipmentItemVM retval = new CreateEquipmentItemVM(equipmentTypesSL);
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

        public EditEquipmentItemVM EditEquipmentVM(Equipment equipment, SelectList equipmentTypesSL)
        {
            EditEquipmentItemVM vm = new EditEquipmentItemVM(equipment, equipmentTypesSL);
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