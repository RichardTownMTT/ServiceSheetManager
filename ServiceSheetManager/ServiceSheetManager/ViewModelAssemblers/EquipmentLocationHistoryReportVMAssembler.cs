//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;
//using ServiceSheetManager.Models;
//using ServiceSheetManager.ViewModels.EquipmentVMs;
//using System.Web.Mvc;

//namespace ServiceSheetManager.ViewModelAssemblers
//{
//    public class EquipmentLocationHistoryReportVMAssembler
//    {
//        public static EquipmentLocationHistoryVM LoadEquipmentOnly(List<Equipment> allEquipmentAndKit)
//        {
//            EquipmentLocationHistoryVM retval = new EquipmentLocationHistoryVM();

//            retval.EquipmentBarcodesAndDesc = CreateEquipmentList(allEquipmentAndKit);

//            return retval;
//        }

//        public static EquipmentLocationHistoryVM LoadEquipmentAndHistory(List<Equipment> allEquipmentKitAndLocations, string selectedEquipmentItemBarcode)
//        {
//            EquipmentLocationHistoryVM retval = new EquipmentLocationHistoryVM();
//            retval.SelectedEquipmentItemBarcode = selectedEquipmentItemBarcode;

//            retval.EquipmentBarcodesAndDesc = CreateEquipmentList(allEquipmentKitAndLocations);

//            retval.AllHistory = CreateHistory(allEquipmentKitAndLocations, selectedEquipmentItemBarcode);

//            return retval;
//        }

//        private static List<EquipmentLocationHistoryReportItem> CreateHistory(List<Equipment> allEquipmentKitAndLocations, string selectedEquipmentItemBarcode)
//        {
//            Equipment equipmentItem = null;
//            List<EquipmentLocationHistoryReportItem> retval = new List<EquipmentLocationHistoryReportItem>();

//            try
//            {
//                equipmentItem = allEquipmentKitAndLocations.Where(e => e.Barcode.Equals(selectedEquipmentItemBarcode)).FirstOrDefault();
//            }
//            catch
//            {
                
//            }
//            if (equipmentItem != null)
//            {
//                retval = CreateEquipmentHistory(equipmentItem);
//                return retval;
//            }
//            else
//            {
//                EquipmentKit kitItem = allEquipmentKitAndLocations.Select(e => e.EquipmentKit).Where(e => e.Barcode.Equals(selectedEquipmentItemBarcode)).FirstOrDefault();
//                if (kitItem != null)
//                {
//                    retval = CreateKitLocationHistory(kitItem);
//                }
//            }  

//            return retval;
//        }

//        private static List<EquipmentLocationHistoryReportItem> CreateEquipmentHistory(Equipment equipmentItem)
//        {
//            List<EquipmentLocation> allLocations = equipmentItem.EquipmentLocations.ToList();

//            if (allLocations.Count == 0)
//            {
//                return new List<EquipmentLocationHistoryReportItem>();
//            }

//            List<EquipmentLocationHistoryReportItem> retval = new List<EquipmentLocationHistoryReportItem>();

//            foreach (var item in allLocations)
//            {
//                string location = EquipmentLocationVMAssembler.GetLocationDescription(item);
//                string scannedBy = item.ScannedUserFirstName + " " + item.ScannedUserSurname;
//                DateTime dtScanned = item.DtScanned;
//                EquipmentLocationHistoryReportItem reportItem = new EquipmentLocationHistoryReportItem(location, scannedBy, dtScanned);
//                retval.Add(reportItem);
//            }

//            return retval;
//        }

//        private static List<EquipmentLocationHistoryReportItem> CreateKitLocationHistory(EquipmentKit kit)
//        {
//            List<EquipmentLocationHistoryReportItem> retval = new List<EquipmentLocationHistoryReportItem>();

//            List<EquipmentLocation> locations = kit.EquipmentLocations.ToList();

//            foreach (var kitItem in locations)
//            {
//                string location = EquipmentLocationVMAssembler.GetLocationDescription(kitItem);
//                string scannedBy = kitItem.ScannedUserFirstName + " " + kitItem.ScannedUserSurname;
//                DateTime dtScanned = kitItem.DtScanned;
//                EquipmentLocationHistoryReportItem item = new EquipmentLocationHistoryReportItem(location, scannedBy, dtScanned);
//                retval.Add(item);
//            }

//            return retval;
//        }

//        private static List<SelectListItem> CreateEquipmentList(List<Equipment> allEquipmentAndKit)
//        {
//            List<SelectListItem> retval = new List<SelectListItem>(); 

//            foreach (var item in allEquipmentAndKit.Where(e => e.EquipmentKitId == null))
//            {
//                string itemDesc = item.Barcode + " - " + item.Description;
//                SelectListItem itemAdd = new SelectListItem() { Text = itemDesc, Value = item.Barcode };
//                retval.Add(itemAdd);
//            }

//            foreach (var kitItem in allEquipmentAndKit.Where(e => e.EquipmentKitId != null).Select(e => e.EquipmentKit).Distinct().ToList())
//            {
//                string kitDesc = kitItem.Barcode + " - " + kitItem.Description;
//                SelectListItem kitItemAdd = new SelectListItem() { Text = kitDesc, Value = kitItem.Barcode };
//                retval.Add(kitItemAdd);
//            }

//            retval = retval.OrderBy(s => s.Value).ToList();

//            return retval;
//        }
//    }
//}