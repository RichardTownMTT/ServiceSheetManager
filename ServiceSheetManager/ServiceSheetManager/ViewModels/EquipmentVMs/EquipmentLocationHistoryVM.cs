//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace ServiceSheetManager.ViewModels.EquipmentVMs
//{
//    public class EquipmentLocationHistoryVM
//    {
//        public EquipmentLocationHistoryVM()
//        {
//            EquipmentBarcodesAndDesc = new List<SelectListItem>();
//            AllHistory = new List<EquipmentLocationHistoryReportItem>();
//        }

//        private List<SelectListItem> equipmentBarcodesAndDesc;
//        private string selectedEquipmentItemBarcode;
//        private List<EquipmentLocationHistoryReportItem> allHistory;

//        [Display(Name = "Equipment Description")]
//        public List<SelectListItem> EquipmentBarcodesAndDesc
//        {
//            get { return equipmentBarcodesAndDesc; }
//            set { equipmentBarcodesAndDesc = value; }
//        }
//        public string SelectedEquipmentItemBarcode
//        {
//            get { return selectedEquipmentItemBarcode; }
//            set { selectedEquipmentItemBarcode = value; }
//        }
//        public List<EquipmentLocationHistoryReportItem> AllHistory
//        {
//            get { return allHistory; }
//            set { allHistory = value; }
//        }
//    }
//}