using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;

namespace ServiceSheetManager.Helpers
{
    public class EquipmentHelpers
    {
        public static bool IsItemCalibrated(EquipmentCalibration calibrationRecord, int? calibrationPeriodYears)
        {
            if (calibrationPeriodYears == null)
            {
                return true;
            }

            if (calibrationRecord == null)
            {
                return false;
            }
            else
            {
                DateTime today = DateTime.Now;
                DateTime calibrationDue = calibrationRecord.DtCalibrated.AddYears(calibrationPeriodYears.Value);
                if (calibrationDue < today)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }

        public static DateTime? CalculateCalibrationDueDate(Equipment item)
        {
            if (item.EquipmentCalibrations.Count == 0)
            {
                return null;
            }

            EquipmentCalibration lastCalibration = item.EquipmentCalibrations.OrderByDescending(c => c.DtCalibrated).FirstOrDefault();
            int? calPeriodYears = item.CalibrationPeriodYears;

            DateTime calDue = lastCalibration.DtCalibrated.AddYears(calPeriodYears.Value);
            return calDue;
        }

        public static DateTime? GetLastCalibratedDate(Equipment item)
        {
            if (item.EquipmentCalibrations.Count == 0)
            {
                return null;
            }

            EquipmentCalibration lastCal = item.EquipmentCalibrations.OrderByDescending(m => m.DtCalibrated).FirstOrDefault();
            return lastCal.DtCalibrated;
        }
    }
}