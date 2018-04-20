using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels.EquipmentVMs
{
    public class EquipmentCalibrationDueReportVM
    {
        public EquipmentCalibrationDueReportVM()
        {
            AllItems = new List<EquipmentCalibrationDueItemVM>();
        }

        private List<EquipmentCalibrationDueItemVM> allItems;

        public List<EquipmentCalibrationDueItemVM> AllItems
        {
            get { return allItems; }
            set { allItems = value; }
        }
    }
}