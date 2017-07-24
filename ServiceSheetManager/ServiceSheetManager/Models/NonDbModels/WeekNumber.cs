using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.Models.NonDbModels
{
    public class WeekNumber
    {
        public int WeekNo { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}