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
    }
}