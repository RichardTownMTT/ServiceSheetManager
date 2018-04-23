using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;
using ServiceSheetManager.Helpers;
using System.ComponentModel.DataAnnotations;
using ServiceSheetManager.ViewModelAssemblers;

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
        private string location;
        private int locationCode;

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
                List<EquipmentLocation> allLocations = item.EquipmentKit.EquipmentLocations.ToList();
                location = EquipmentLocationVMAssembler.GetLocationDescription(allLocations);
                locationCode = EquipmentLocationVMAssembler.GetLocationCode(allLocations);
            }
            else
            {
                Barcode = item.Barcode;
                KitDescription = "N/A";
                List<EquipmentLocation> locations = item.EquipmentLocations.ToList();
                location = EquipmentLocationVMAssembler.GetLocationDescription(locations);
                locationCode = EquipmentLocationVMAssembler.GetLocationCode(locations);
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

        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        public string GetRowCss
        {
            get
            {
                if (locationCode == EquipmentLocationVMAssembler.AWAY_FOR_CALIBRATION_INT)
                {
                    return "success";
                }
                return "";
            }
        }
    }
}