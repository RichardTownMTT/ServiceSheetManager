using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModelAssemblers;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class EquipmentIndexKitItemVM
    {
        private int id;
        private string barcode;
        private string description;
        private string currentLocation;
        private string equipmentType;

        public EquipmentIndexKitItemVM(EquipmentKit kitEquipment)
        {
            id = kitEquipment.Id;
            Barcode = kitEquipment.Barcode;
            Description = kitEquipment.Description;
            if (kitEquipment.EquipmentType != null)
            {
                EquipmentType = kitEquipment.EquipmentType.Description;
            }
            else
            {
                EquipmentType = "Type Not Set";
            }
            

            EquipmentLocation currentLocation = kitEquipment.EquipmentLocations.OrderByDescending(e => e.DtScanned).FirstOrDefault();

            if (currentLocation != null)
            {
                CurrentLocation = EquipmentLocationVMAssembler.GetLocationDescription(currentLocation);
            }
            else
            {
                CurrentLocation = "Location Not Set";
            }
        }

        public int Id
        {
            get { return id; }
        }
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        [Display(Name = "Current Location")]
        public string CurrentLocation
        {
            get { return currentLocation; }
            set { currentLocation = value; }
        }
        [Display(Name = "Equipment Type")]
        public string EquipmentType
        {
            get { return equipmentType; }
            set { equipmentType = value; }
        }
    }
}