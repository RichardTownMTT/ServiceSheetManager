using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class CreateEquipmentItemVM
    {
        private string barcode;
        private string description;
        private string serialNumber;
        private int? calibrationPeriodYears;
        private SelectList equipmentTypes;
        private string equipmentTypeSelected;

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
            get { return calibrationPeriodYears; }
            set { calibrationPeriodYears = value; }
        }
        [Display(Name = "Equipment Type")]
        public SelectList EquipmentTypes
        {
            get { return equipmentTypes; }
            set
            { equipmentTypes = value; }
        }
        public string EquipmentTypeSelected
        {
            get { return equipmentTypeSelected; }
            set { equipmentTypeSelected = value; }
        }
    }
}