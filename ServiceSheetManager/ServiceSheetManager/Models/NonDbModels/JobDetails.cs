using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.Models.NonDbModels
{
    public class JobDetails
    {
        public JobDetails(string JobNumber, string CustomerSet, double OnsiteTime, double TravelTime)
        {
            MttJobNumber = JobNumber;
            Customer = CustomerSet;
            TotalTimeOnsite = OnsiteTime;
            TotalTravelTime = TravelTime;
        }

        public string MttJobNumber { get; set; }
        public string Customer { get; set; }
        public double TotalTimeOnsite { get; set; }
        public double TotalTravelTime { get; set; }
    }
}