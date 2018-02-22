using System;
using System.Collections.Generic;
using System.Linq;
using ServiceSheetManager.Models;
using System.ComponentModel.DataAnnotations;
using ServiceSheetManager.ViewModelAssemblers;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class EquipmentIndexEquipmentItemVM
    {
        private int id;
        private string barcode;
        private string description;
        private string currentLocation;
        private string equipmentTypeDescription;

        public EquipmentIndexEquipmentItemVM(Equipment equipmentModel)
        {
            this.id = equipmentModel.Id;
            this.Barcode = equipmentModel.Barcode;
            this.Description = equipmentModel.Description;

            if (equipmentModel.EquipmentType != null)
            {
                this.equipmentTypeDescription = equipmentModel.EquipmentType.Description;
            }
            else
            {
                this.equipmentTypeDescription = "Not Set";
                System.Diagnostics.Trace.TraceError("Equipment Type not loaded! - Equipment Model id = " + equipmentModel.Id.ToString());
            }

            EquipmentLocation currentLocation = equipmentModel.EquipmentLocations.OrderByDescending(e => e.DtScanned).FirstOrDefault();

            if (currentLocation  != null)
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
        public string EquipmentTypeDescription
        {
            get { return equipmentTypeDescription; }
            set { equipmentTypeDescription = value; }
        }
    }
}