using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;
using ServiceSheetManager.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class EquipmentCalibrationDueItemVM
    {
        private string barcode;
        private DateTime? lastCalibrated;
        private string description;
        private string kitDescription;
        private int calibrationPeriod;
        private DateTime? calibrationDue;

        public EquipmentCalibrationDueItemVM(Equipment item)
        {
            if (!item.CalibrationPeriodYears.HasValue)
            {
                throw new Exception("Calibrated items only in this report");
            }

            if (item.EquipmentKitId != null)
            {
                Barcode = item.EquipmentKit.Barcode;
                KitDescription = item.EquipmentKit.Description;
            }
            else
            {
                Barcode = item.Barcode;
                KitDescription = "N/A";
            }

            CalibrationDue = EquipmentHelpers.CalculateCalibrationDueDate(item);
            LastCalibrated = EquipmentHelpers.GetLastCalibratedDate(item);
            Description = item.Description;
            CalibrationPeriod = item.CalibrationPeriodYears.Value;
        }

        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }
        public DateTime? LastCalibrated
        {
            get { return lastCalibrated; }
            set { lastCalibrated = value; }
        }
        [Display(Name = "Last Calibrated")]
        public string LastCalibratedStr
        {
            get
            {
                if (LastCalibrated.HasValue)
                {
                    return LastCalibrated.Value.ToShortDateString();
                }
                else
                {
                    return "No calibration record found";
                }
            }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        [Display(Name = "Kit")]
        public string KitDescription
        {
            get { return kitDescription; }
            set { kitDescription = value; }
        }
        [Display(Name = "Calibration Period (Years)")]
        public int CalibrationPeriod
        {
            get { return calibrationPeriod; }
            set { calibrationPeriod = value; }
        }
        public DateTime? CalibrationDue
        {
            get { return calibrationDue; }
            set { calibrationDue = value; }
        }
        [Display(Name = "Calibration Due")]
        public string CalibrationDueStr
        {
            get
            {
                if (CalibrationDue.HasValue)
                {
                    return CalibrationDue.Value.ToShortDateString();
                }
                else
                {
                    return "No calibration record found";
                }
            }
        }
    }
}