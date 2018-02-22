using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class EquipmentIndexVM
    {
        private List<EquipmentIndexEquipmentItemVM> allEquipmentNotInKit;
        private List<EquipmentIndexKitItemVM> allKits;

        //Filter options
        private int selectedEquipmentTypeId;
        private SelectList equipmentTypes;

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
        [Display(Name = "Equipment Type")]
        public int SelectedEquipmentTypeId
        {
            get { return selectedEquipmentTypeId; }
            set { selectedEquipmentTypeId = value; }
        }
        public SelectList EquipmentTypes
        {
            get { return equipmentTypes; }
            set { equipmentTypes = value; }
        }
    }
}