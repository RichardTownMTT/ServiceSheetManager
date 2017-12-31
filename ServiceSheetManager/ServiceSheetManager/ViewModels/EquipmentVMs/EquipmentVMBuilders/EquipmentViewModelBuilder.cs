using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels.EquipmentVMs.EquipmentVMBuilders
{
    public class EquipmentViewModelBuilder
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
    }
}