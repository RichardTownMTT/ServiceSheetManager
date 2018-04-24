using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class CreateEquipmentCalibrationVM
    {
        private List<SelectListItem> equipmentBarcodesAndDesc;
        private string selectedEquipmentItem;
        private string equipmentDescription;
        private string serialNumber;
        private string calibrationPeriod;
        private int idSelected;
        private string certificateNo;
        private bool passedCalibration;
        private DateTime dtPassedCal;
        private bool equipmentLoaded;

        public CreateEquipmentCalibrationVM()
        {
            EquipmentBarcodesAndDesc = new List<SelectListItem>();
            DtPassedCal = new DateTime();
            EquipmentLoaded = false;
        }

        [Display(Name = "Equipment Description")]
        public List<SelectListItem> EquipmentBarcodesAndDesc
        {
            get { return equipmentBarcodesAndDesc; }
            set { equipmentBarcodesAndDesc = value; }
        }
        public string SelectedEquipmentItem
        {
            get { return selectedEquipmentItem; }
            set { selectedEquipmentItem = value; }
        }
        [Display(Name = "Description")]
        public string EquipmentDescription
        {
            get { return equipmentDescription; }
            set { equipmentDescription = value; }
        }
        public int IdSelected
        {
            get { return idSelected; }
            set { idSelected = value; }
        }
        [Display(Name = "Serial Number")]
        public string SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }
        [Display(Name = "Calibration Period (Years)")]
        public string CalibrationPeriod
        {
            get { return calibrationPeriod; }
            set { calibrationPeriod = value; }
        }
        [Display(Name = "Calibration Passed")]
        public bool PassedCalibration
        {
            get { return passedCalibration; }
            set { passedCalibration = value; }
        }
        [Display(Name = "Date Calibration Passed")]
        [Required]
        public DateTime DtPassedCal
        {
            get { return dtPassedCal; }
            set { dtPassedCal = value; }
        }
        public bool EquipmentLoaded
        {
            get { return equipmentLoaded; }
            set { equipmentLoaded = value; }
        }
        [Display(Name = "Certificate Number")]
        public string CertificateNo
        {
            get { return certificateNo; }
            set { certificateNo = value; }
        }
    }
}