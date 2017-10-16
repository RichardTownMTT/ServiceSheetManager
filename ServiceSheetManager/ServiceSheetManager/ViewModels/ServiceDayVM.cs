using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServiceSheetManager.ViewModels
{
    public class ServiceDayVM
    {
        public ServiceDayVM()
        {

        }

        //Constructor for the service day vm from the db model
        public ServiceDayVM(ServiceDay dayModel)
        {
            this.Id = dayModel.Id;
            this.TravelStartTime = dayModel.TravelStartTime;
            this.ArrivalOnsiteTime = dayModel.ArrivalOnsiteTime;
            this.DepartureSiteTime = dayModel.DepartureSiteTime;
            this.TravelEndTime = dayModel.TravelEndTime;
            this.Mileage = dayModel.Mileage;
            this.DailyAllowance = dayModel.DailyAllowance;
            this.OvernightAllowance = dayModel.OvernightAllowance;
            this.BarrierPayment = dayModel.BarrierPayment;
            this.TravelToSiteTime = dayModel.TravelToSiteTime;
            this.TotalOnsiteTime = dayModel.TotalOnsiteTime;
            this.TravelFromSiteTime = dayModel.TravelFromSiteTime;
            this.TotalTravelTime = dayModel.TotalTravelTime;
            this.DailyReport = dayModel.DailyReport;
            this.PartsSuppliedToday = dayModel.PartsSuppliedToday;
            this.DtReport = dayModel.DtReport;
            this.ServiceSheetId = dayModel.ServiceSheetId;
        }

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

        public Nullable<int> ServiceSheetId { get; set; }


        public string ReportDayOfWeekName
        {
            get { return DtReport.DayOfWeek.ToString(); }
        }

        public void UpdateDay(ServiceDay day)
        {
            day.ArrivalOnsiteTime = this.ArrivalOnsiteTime;
            day.BarrierPayment = this.BarrierPayment;
            day.DailyAllowance = this.DailyAllowance;
            day.DailyReport = this.DailyReport;
            day.DepartureSiteTime = this.DepartureSiteTime;
            day.DtReport = this.DtReport;
            day.Mileage = this.Mileage;
            day.OvernightAllowance = this.OvernightAllowance;
            day.PartsSuppliedToday = this.PartsSuppliedToday;
            day.TotalOnsiteTime = this.TotalOnsiteTime;
            day.TotalTravelTime = this.TotalTravelTime;
            day.TravelEndTime = this.TravelEndTime;
            day.TravelFromSiteTime = this.TravelFromSiteTime;
            day.TravelStartTime = this.TravelStartTime;
            day.TravelToSiteTime = this.TravelToSiteTime;
        }

        public static List<ServiceDay> CreateServiceDaysFromVM(ServiceSheetApprovalVM vm, ServiceSheet sheetToSave)
        {
            //Creates the service day entity to be saved to the database
            List<ServiceDay> retval = new List<ServiceDay>();

            foreach (var dayVM in vm.ServiceDayModels)
            {
                ServiceDay dayModel = new ServiceDay
                {
                    DtReport = dayVM.ServiceDayEntity.DtReport,
                    BarrierPayment = dayVM.ServiceDayEntity.BarrierPayment,
                    DailyAllowance = dayVM.ServiceDayEntity.DailyAllowance,
                    DailyReport = dayVM.ServiceDayEntity.DailyReport,
                    Mileage = dayVM.ServiceDayEntity.Mileage,
                    OvernightAllowance = dayVM.ServiceDayEntity.OvernightAllowance,
                    PartsSuppliedToday = dayVM.ServiceDayEntity.PartsSuppliedToday,
                    ServiceSheet = sheetToSave,
                    TotalOnsiteTime = dayVM.ServiceDayEntity.TotalOnsiteTime,
                    TotalTravelTime = dayVM.ServiceDayEntity.TotalTravelTime,
                    TravelFromSiteTime = dayVM.ServiceDayEntity.TravelFromSiteTime,
                    TravelToSiteTime = dayVM.ServiceDayEntity.TravelToSiteTime
                };
                //Set the date portion of the times to the report date
                dayModel.ArrivalOnsiteTime = dayModel.DtReport.Date + dayVM.ServiceDayEntity.ArrivalOnsiteTime.TimeOfDay;
                dayModel.DepartureSiteTime = dayModel.DtReport.Date + dayVM.ServiceDayEntity.DepartureSiteTime.TimeOfDay;
                dayModel.TravelEndTime = dayModel.DtReport.Date + dayVM.ServiceDayEntity.TravelEndTime.TimeOfDay;
                dayModel.TravelStartTime = dayModel.DtReport.Date + dayVM.ServiceDayEntity.TravelStartTime.TimeOfDay;

                retval.Add(dayModel);
            }


            return retval;
        }
    }
}