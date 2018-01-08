using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class EquipmentIndexVM
    {
        private List<EquipmentIndexEquipmentItemVM> allEquipmentNotInKit;
        private List<EquipmentIndexKitItemVM> allKits;

        public EquipmentIndexVM()
        {
            AllEquipmentNotInKitItems = new List<EquipmentIndexEquipmentItemVM>();
            AllKits = new List<EquipmentIndexKitItemVM>();
        }

        public List<EquipmentIndexEquipmentItemVM> AllEquipmentNotInKitItems
        {
            get { return allEquipmentNotInKit; }
            set { allEquipmentNotInKit = value; }
        }
        public List<EquipmentIndexKitItemVM> AllKits
        {
            get { return allKits; }
            set { allKits = value; }
        }
    }
}