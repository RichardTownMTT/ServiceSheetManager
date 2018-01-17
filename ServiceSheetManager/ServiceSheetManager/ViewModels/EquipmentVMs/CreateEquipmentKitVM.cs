using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceSheetManager.Models;
using System.Threading.Tasks;
using ServiceSheetManager.ViewModelAssemblers;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class CreateEquipmentKitVM
    {
        public CreateEquipmentKitVM()
        {
        }

        public CreateEquipmentKitVM(SelectList equipmentTypesSL)
        {
            this.EquipmentTypes = equipmentTypesSL;
            this.EquipmentItems = new List<CreateEquipmentItemVM>();
        }

        private string barcode;
        private string description;
        private List<CreateEquipmentItemVM> equipmentItems;
        private string selectedEquipmentTypeId;
        private SelectList equipmentTypes;

        [Required]
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }
        [Required]
        public  string Description
        {
            get { return description; }
            set { description = value; }
        }
        public List<CreateEquipmentItemVM> EquipmentItems
        {
            get { return equipmentItems; }
            set
            { equipmentItems = value; }
        }
        public SelectList EquipmentTypes
        {
            get { return equipmentTypes; }
            set { equipmentTypes = value; }
        }
        [Display(Name = "Equipment Type")]
        public string SelectedEquipmentTypeId
        {
            get { return selectedEquipmentTypeId; }
            set { selectedEquipmentTypeId = value; }
        }

        public async Task RepopulateSelectLists(ServiceSheetsEntities db, int? selectedId)
        {
            SelectList equipmentTypes = await EquipmentTypeVMAssembler.GetAllTypes(db, selectedId);
            this.EquipmentTypes = equipmentTypes;
        }
    }
}