using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;

namespace ServiceSheetManager.ViewModelAssemblers
{
    public class EquipmentLocationVMAssembler
    {
        public const string ASSIGN_TO_ME = "Assign to me";
        public const string RETURN_TO_UNIT = "Return to Unit";
        public const string AWAY_FOR_CALIBRATION = "Away for Calibration";
        public const int ASSIGN_TO_ME_INT = 1;
        public const int RETURN_TO_UNIT_INT = 2;
        public const int AWAY_FOR_CALIBRATION_INT = 3;

        public static int SetLocationCodeForLocation(string location)
        {
            int retval;
            switch (location)
            {
                case ASSIGN_TO_ME:
                    retval = ASSIGN_TO_ME_INT;
                    break;
                case RETURN_TO_UNIT:
                    retval = RETURN_TO_UNIT_INT;
                    break;
                case AWAY_FOR_CALIBRATION:
                    retval = AWAY_FOR_CALIBRATION_INT;
                    break;
                default:
                    retval = -1;
                    break;
            }

            return retval;
        }

        public static int GetLocationCode(List<EquipmentLocation> allLocations)
        {
            if (allLocations.Count == 0)
            {
                return -1;
            }

            EquipmentLocation current = allLocations.OrderByDescending(l => l.DtScanned).FirstOrDefault();
            return current.LocationCode;
        }

        public static string GetLocationDescription(List<EquipmentLocation> allLocations)
        {
            if (allLocations.Count == 0)
            {
                return "Location Not Set";
            }

            EquipmentLocation latest = allLocations.OrderByDescending(l => l.DtScanned).FirstOrDefault();

            if (latest != null)
            {
                return GetLocationDescription(latest);
            }
            else
            {
                return "Location Not Set";
            }
        }

        public static string GetLocationDescription(EquipmentLocation currentLocation)
        {
            string retval = "";
            switch (currentLocation.LocationCode)
            {
                case ASSIGN_TO_ME_INT:
                    retval = currentLocation.ScannedUserFirstName + " " + currentLocation.ScannedUserSurname;
                    break;
                case RETURN_TO_UNIT_INT:
                    retval = "Unit";
                    break;
                case AWAY_FOR_CALIBRATION_INT:
                    retval = AWAY_FOR_CALIBRATION;
                    break;
                default:
                    retval = "Unknown Location Code";
                    break;
            }

            return retval;
        }
    }
}