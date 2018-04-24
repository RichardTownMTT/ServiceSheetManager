using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModels.EquipmentVMs;
using System.Web.Mvc;

namespace ServiceSheetManager.ViewModelAssemblers
{
    public class EquipmentCalibrationVMAssembler
    {
        public CreateEquipmentCalibrationVM CreateEquipmentSearchView(List<Equipment> allEquipmentAndKits, CreateEquipmentCalibrationVM equipmentCalVMReturned)
        {
            CreateEquipmentCalibrationVM vm = new CreateEquipmentCalibrationVM();

            vm.EquipmentBarcodesAndDesc = createEquipmentSelectList(allEquipmentAndKits);

            int equipmentIdSelected;
            try
            {
                equipmentIdSelected = int.Parse(equipmentCalVMReturned.SelectedEquipmentItem);
            }
            catch (FormatException ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                equipmentIdSelected = 0;
            }
            catch (ArgumentNullException nEx)
            {
                equipmentIdSelected = 0;
            }

            Equipment equipmentSelected = null;
            if (equipmentIdSelected != 0)
            {
                equipmentSelected = allEquipmentAndKits.Where(e => e.Id == equipmentIdSelected).FirstOrDefault();
            }

            if (equipmentSelected != null)
            {
                vm.EquipmentDescription = equipmentSelected.Description;
                vm.IdSelected = equipmentSelected.Id;
                vm.CalibrationPeriod = equipmentSelected.CalibrationPeriodYears.ToString();
                vm.SerialNumber = equipmentSelected.SerialNumber;
                vm.EquipmentLoaded = true;
            }

            return vm;
        }

        private List<SelectListItem> createEquipmentSelectList(List<Equipment> allEquipmentAndKits)
        {
            List<SelectListItem> retval = new List<SelectListItem>();

            foreach (var equipment in allEquipmentAndKits)
            {
                if (equipment.EquipmentKitId != null)
                {
                    string kitDesc = createKitDescription(equipment);
                    SelectListItem item = new SelectListItem() { Value = kitDesc, Text = equipment.Id.ToString() };
                    retval.Add(item);
                }
                else
                {
                    string kitDesc = createEquipmentDescription(equipment);
                    SelectListItem item = new SelectListItem() { Value = kitDesc, Text = equipment.Id.ToString() };
                    retval.Add(item);
                }
            }

            retval = retval.OrderBy(e => e.Value).ToList();
            return retval;
        }

        private string createEquipmentDescription(Equipment equipment)
        {
            string retval = equipment.Barcode + " - " + equipment.Description;
            return retval;
        }

        private string createKitDescription(Equipment equipment)
        {
            string retval = equipment.EquipmentKit.Barcode + " - " + equipment.Description;
            return retval;
        }
    }
}