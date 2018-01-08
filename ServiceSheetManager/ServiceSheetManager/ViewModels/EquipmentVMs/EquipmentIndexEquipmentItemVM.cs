using System;
using System.Collections.Generic;
using System.Linq;
using ServiceSheetManager.Models;
using System.ComponentModel.DataAnnotations;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class EquipmentIndexEquipmentItemVM
    {
        private int id;
        private string barcode;
        private string description;
        private string currentLocation;

        public EquipmentIndexEquipmentItemVM(Equipment equipmentModel)
        {
            this.id = equipmentModel.Id;
            this.Barcode = equipmentModel.Barcode;
            this.Description = equipmentModel.Description;

            if (equipmentModel.EquipmentLocations.FirstOrDefault() != null)
            {
                CurrentLocation = equipmentModel.EquipmentLocations.FirstOrDefault().ScannedUserFirstName.ToString();
            }
            else
            {
                CurrentLocation = "Error";
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
    }
}