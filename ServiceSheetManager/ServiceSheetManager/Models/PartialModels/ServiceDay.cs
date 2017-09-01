using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.Models
{
    [MetadataType(typeof(ServiceDay.ServiceDayMetaData))]
    public partial class ServiceDay
    {
        internal sealed class ServiceDayMetaData
        {
            [Editable(false)]
            [Required]
            public int Id { get; set; }

            [Required(ErrorMessage = "Travel start time is required.")]
            [Display(Name = "Travel Start Time")]
            [DataType(DataType.Time)]
            public System.DateTime TravelStartTime { get; set; }

            [Required(ErrorMessage = "Arrival time onsite is required")]
            [Display(Name = "Arrival Onsite Time")]
            [DataType(DataType.Time)]
            public System.DateTime ArrivalOnsiteTime { get; set; }

            [Required(ErrorMessage = "Departure site time is required")]
            [Display(Name = "Departure From Site Time")]
            [DataType(DataType.Time)]
            public System.DateTime DepartureSiteTime { get; set; }

            [Required(ErrorMessage = "Travel end time is required")]
            [Display(Name = "Travel End Time")]
            [DataType(DataType.Time)]
            public System.DateTime TravelEndTime { get; set; }

            [Required(ErrorMessage = "Mileage is required")]
            public int Mileage { get; set; }

            [Required(ErrorMessage = "Daily alllowance is required")]
            [Display(Name = "Daily Allowance")]
            [Range(0, 1, ErrorMessage = "Allowed values are 1 or 0")]
            public int DailyAllowance { get; set; }

            [Required(ErrorMessage = "Overnight allowance is required")]
            [Display(Name = "Overnight Allowance")]
            [Range(0, 1, ErrorMessage = "Allowed values are 1 or 0")]
            public int OvernightAllowance { get; set; }

            [Required(ErrorMessage = "Barrier payment is required")]
            [Display(Name = "Barrier Payment")]
            [Range(0, 1, ErrorMessage = "Allowed values are 1 or 0")]
            public int BarrierPayment { get; set; }

            [Required]
            [Editable(false)]
            [Display(Name = "Travel Time To Site")]
            public double TravelToSiteTime { get; set; }

            [Required]
            [Editable(false)]
            [Display(Name = "Travel Time From Site")]
            public double TravelFromSiteTime { get; set; }

            [Required]
            [Editable(false)]
            [Display(Name = "Total Travel Time")]
            public double TotalTravelTime { get; set; }

            [Required]
            [Editable(false)]
            [Display(Name = "Total Time Onsite")]
            public double TotalOnsiteTime { get; set; }

            [Required(ErrorMessage = "Daily report is required", AllowEmptyStrings = false)]
            [Display(Name = "Daily Report")]
            public string DailyReport { get; set; }

            [Display(Name = "Parts Supplied Today")]
            public string PartsSuppliedToday { get; set; }

            [Required(ErrorMessage = "Report date is required")]
            [Display(Name = "Report Date")]
            [DataType(DataType.Date)]
            public System.DateTime DtReport { get; set; }

            [Display(Name = "Day")]
            [Editable(false)]
            public string ReportDayOfWeek { get; }

            public Nullable<int> ServiceSheetId { get; set; }
        }
        
        public string ReportDayOfWeek
        {
            get { return DtReport.DayOfWeek.ToString(); }
        }
    }
}