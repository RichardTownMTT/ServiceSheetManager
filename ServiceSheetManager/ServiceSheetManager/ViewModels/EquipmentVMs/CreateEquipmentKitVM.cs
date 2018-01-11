using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class CreateEquipmentKitVM
    {
        public CreateEquipmentKitVM()
        {
            EquipmentItems = new List<CreateEquipmentItemVM>();
        }

        private string barcode;
        private string description;
        private List<CreateEquipmentItemVM> equipmentItems;

        [Required]
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }
        [Required]
        public  string Description
        {
            get { return description; }
            set { description = value; }
        }
        public List<CreateEquipmentItemVM> EquipmentItems
        {
            get { return equipmentItems; }
            set { equipmentItems = value; }
        }

    }
}