using System;
using System.Collections.Generic;
using System.Linq;
using ServiceSheetManager.Models;
using System.ComponentModel.DataAnnotations;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class DisplayEquipmentListItemVM
    {
        private int id;
        private string barcode;
        private string description;
        private string serialNumber;
        private int? calibrationPeriodDays;


        public DisplayEquipmentListItemVM(Equipment equipmentModel)
        {
            this.id = equipmentModel.Id;
            this.Barcode = equipmentModel.Barcode;
            this.Description = equipmentModel.Description;
            this.SerialNumber = equipmentModel.SerialNumber;
            this.CalibrationPeriodDays = equipmentModel.CalibrationPeriodDays; 
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
        [Display(Name = "Serial Number")]
        public string SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }
        [Display(Name = "Calibration Period (Days)")]
        public Nullable<int> CalibrationPeriodDays
        {
            get { return calibrationPeriodDays; }
            set { calibrationPeriodDays = value; }
        }
    }
}