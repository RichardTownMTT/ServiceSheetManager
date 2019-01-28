//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;

//namespace ServiceSheetManager.ViewModels.EquipmentVMs
//{
//    public class EquipmentLocationHistoryReportItem
//    {
//        private string location;
//        private string scannedBy;
//        private DateTime dateAssigned;
//        private DateTime dtScanned;

//        public EquipmentLocationHistoryReportItem(string location, string scannedBy, DateTime dtScanned)
//        {
//            this.location = location;
//            this.scannedBy = scannedBy;
//            this.dtScanned = dtScanned;
//        }

//        public string Location
//        {
//            get { return location; }
//            set { location = value; }
//        }
//        [Display(Name = "Scanned By")]
//        public string ScannedBy
//        {
//            get { return scannedBy; }
//            set { scannedBy = value; }
//        }
//        public DateTime DateAssigned
//        {
//            get { return dateAssigned; }
//            set { dateAssigned = value; }
//        }
//        public string DisplayDateAssigned
//        {
//            get { return DateAssigned.ToShortDateString(); }
//        }
//    }
//}