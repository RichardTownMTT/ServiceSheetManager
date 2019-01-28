using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModelAssemblers;
using ServiceSheetManager.Helpers;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class EquipmentIndexKitItemVM
    {
        private int id;
        private string barcode;
        private string description;
        private string currentLocation;
        private string equipmentType;
        private bool allCalibrated;

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

            AllCalibrated = true;

            foreach (var item in kitEquipment.Equipments)
            {
                int? calibrationPeriodYears = item.CalibrationPeriodYears;

                EquipmentCalibration calibrationRecord = item.EquipmentCalibrations.Where(e => e.Passed == true).OrderByDescending(c => c.DtCalibrated).FirstOrDefault();
                bool calibrated = EquipmentHelpers.IsItemCalibrated(calibrationRecord, calibrationPeriodYears);

                if (!calibrated)
                {
                    AllCalibrated = false;
                }
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
        public bool AllCalibrated
        {
            get { return allCalibrated; }
            set { allCalibrated = value; }
        }
        [Display(Name = "Calibration Status")]
        public String CalibrationText
        {
            get
            {
                if (AllCalibrated)
                {
                    return "Calibrated";
                }
                else
                {
                    return "Out of Calibration";
                }
            }
        }

        public string GetCalibrationCssClass
        {
            get
            {
                if (CurrentLocation.Equals(EquipmentLocationVMAssembler.AWAY_FOR_CALIBRATION))
                {
                    return "success";
                }

                if (!AllCalibrated)
                {
                    return "danger";
                }
                return "";
            }
        }
    }
}