using ServiceSheetManager.ViewModels.EquipmentVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;
using System.Web.Mvc;

namespace ServiceSheetManager.ViewModelAssemblers
{
    public class EquipmentKitVMAssembler
    {
        public CreateEquipmentKitVM Create(SelectList equipmentTypesSL)
        {
            return new CreateEquipmentKitVM(equipmentTypesSL);
        }

        public EquipmentKit Map(CreateEquipmentKitVM equipmentVM)
        {
            EquipmentKit retval = new EquipmentKit()
            {
                Barcode = equipmentVM.Barcode,
                Description = equipmentVM.Description,
                EquipmentTypeId = int.Parse(equipmentVM.SelectedEquipmentTypeId)
            };
            return retval;
        }

        public DisplayEquipmentKitVM Display(EquipmentKit equipmentKit)
        {
            DisplayEquipmentKitVM retval = new DisplayEquipmentKitVM(equipmentKit);
            return retval;
        }
    }
}