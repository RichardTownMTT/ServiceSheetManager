using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class EquipmentListVM
    {
        public List<DisplayEquipmentListItemVM> AllItems;

        public EquipmentListVM()
        {
            AllItems = new List<DisplayEquipmentListItemVM>();
        }
    }
}