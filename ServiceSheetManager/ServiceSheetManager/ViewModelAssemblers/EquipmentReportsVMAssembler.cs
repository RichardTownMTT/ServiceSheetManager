using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModels.EquipmentVMs;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ServiceSheetManager.ViewModelAssemblers
{
    public class EquipmentReportsVMAssembler
    {
        public static async Task<EquipmentCalibrationDueReportVM> GenerateReportVM(IQueryable<Equipment> allEquipment)
        {
            List<Equipment> equipmentItems = await allEquipment.Where(e => e.CalibrationPeriodYears != null).Include(e => e.EquipmentCalibrations).Include(e => e.EquipmentKit).ToListAsync();

            EquipmentCalibrationDueReportVM retval = new EquipmentCalibrationDueReportVM();

            foreach (var item in equipmentItems)
            {
                EquipmentCalibrationDueItemVM itemToAdd = new EquipmentCalibrationDueItemVM(item);

                //Add to report if calibration day is before today or hasn't been calibrated
                if (itemToAdd.CalibrationDue.HasValue)
                {
                    if (itemToAdd.CalibrationDue.Value < DateTime.Now)
                    {
                        retval.AllItems.Add(itemToAdd);
                    } 
                }
                else if (!itemToAdd.CalibrationDue.HasValue)
                {
                    retval.AllItems.Add(itemToAdd);
                }
                
            }

            return retval;
        }
    }
}