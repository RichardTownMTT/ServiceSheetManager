using ServiceSheetManager.ViewModels.EquipmentVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;

namespace ServiceSheetManager.ViewModelAssemblers
{
    public class EquipmentKitVMAssembler
    {
        public CreateEquipmentKitVM Create()
        {
            return new CreateEquipmentKitVM();
        }

        public EquipmentKit Map(CreateEquipmentKitVM equipmentVM)
        {
            EquipmentKit retval = new EquipmentKit()
            {
                Barcode = equipmentVM.Barcode,
                Description = equipmentVM.Description
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