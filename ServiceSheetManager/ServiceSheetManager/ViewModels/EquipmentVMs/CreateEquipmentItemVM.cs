using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class CreateEquipmentItemVM
    {
        private string barcode;
        private string description;
        private string serialNumber;
        private int? calibrationPeriodDays;

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
        [Display(Name = "Calibration Period (Days)")]
        public Nullable<int> CalibrationPeriodDays
        {
            get { return calibrationPeriodDays; }
            set { calibrationPeriodDays = value; }
        }
    }
}