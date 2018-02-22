using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;
using System.Web.Mvc;
using System.Threading.Tasks;
using ServiceSheetManager.ViewModelAssemblers;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class EditEquipmentItemVM
    {
        public EditEquipmentItemVM()
        {
        }

        public EditEquipmentItemVM(Equipment equipment, SelectList equipmentTypesSL)
        {
            this.id = equipment.Id;
            this.Barcode = equipment.Barcode;
            this.Description = equipment.Description;
            this.SerialNumber = equipment.SerialNumber;
            this.CalibrationPeriodYears = equipment.CalibrationPeriodYears;

            if (equipment.EquipmentType != null)
            {
                this.EquipmentTypeSelected = equipment.EquipmentType.Id.ToString();
            }
            else
            {
                throw new Exception("Equipment Type not loaded! for equipment model - " + equipment.Id.ToString());
            }

            this.EquipmentTypes = equipmentTypesSL;
            
        }

        private int id;
        private string barcode;
        private string description;
        private string serialNumber;
        private Nullable<int> calibrationPeriodDays;
        private SelectList equipmentTypes;
        private string equipmentTypeSelected;

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

        public async Task RepopulateSelectLists(ServiceSheetsEntities db)
        {
            SelectList equipmentTypesSL = await EquipmentTypeVMAssembler.GetAllTypes(db, int.Parse(this.EquipmentTypeSelected));
            this.EquipmentTypes = equipmentTypesSL;
        }
    }
}