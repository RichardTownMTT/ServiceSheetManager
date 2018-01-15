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
        public const int ASSIGN_TO_ME_INT = 1;
        public const int RETURN_TO_UNIT_INT = 2;

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
                default:
                    retval = -1;
                    break;
            }

            return retval;
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
                default:
                    retval = "Unknown Location Code";
                    break;
            }

            return retval;
        }
    }
}