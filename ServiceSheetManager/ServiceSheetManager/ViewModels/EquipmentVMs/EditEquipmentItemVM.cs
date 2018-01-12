using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class EditEquipmentItemVM
    {
        public EditEquipmentItemVM()
        {
        }

        public EditEquipmentItemVM(Equipment equipment)
        {
            this.id = equipment.Id;
            this.Barcode = equipment.Barcode;
            this.Description = equipment.Description;
            this.SerialNumber = equipment.SerialNumber;
            this.CalibrationPeriodYears = equipment.CalibrationPeriodYears;
        }

        private int id;
        private string barcode;
        private string description;
        private string serialNumber;
        private Nullable<int> calibrationPeriodDays;

        [Required]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        [Required]
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }
        [Required]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        [Display(Name = "Serial Number")]
        [Required]
        public string SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }
        [Required]
        [Display(Name = "Calibration Period (Years)")]
        public Nullable<int> CalibrationPeriodYears
        {
            get { return calibrationPeriodDays; }
            set { calibrationPeriodDays = value; }
        }
    }
}