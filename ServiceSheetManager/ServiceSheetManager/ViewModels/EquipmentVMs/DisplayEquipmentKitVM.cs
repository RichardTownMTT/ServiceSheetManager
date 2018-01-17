using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class DisplayEquipmentKitVM
    {
        private int id;
        private string description;
        private string barcode;
        private List<DisplayEquipmentItemVM> equipmentItems;
        private string equipmentType;

        public DisplayEquipmentKitVM(EquipmentKit kit)
        {
            this.EquipmentItems = new List<DisplayEquipmentItemVM>();

            this.Id = kit.Id;
            this.Description = kit.Description;
            this.Barcode = kit.Barcode;
            
            if(kit.EquipmentType != null)
            {
                this.EquipmentType = kit.EquipmentType.Description;
            }
            else
            {
                this.equipmentType = "Equipment Type not set";
            }

            foreach (var equipment in kit.Equipments)
            {
                var equipmentToAdd = new DisplayEquipmentItemVM(equipment);
                this.EquipmentItems.Add(equipmentToAdd);
            }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }
        public List<DisplayEquipmentItemVM> EquipmentItems
        {
            get { return equipmentItems; }
            set { equipmentItems = value; }
        }
        public string EquipmentType
        {
            get { return equipmentType; }
            set { equipmentType = value; }
        }
    }
}