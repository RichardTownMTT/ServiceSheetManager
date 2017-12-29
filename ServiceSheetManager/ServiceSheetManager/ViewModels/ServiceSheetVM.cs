using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ServiceSheetManager.Models;
using ServiceSheetManager.Helpers;

namespace ServiceSheetManager.ViewModels
{
    //View Model for the Service Sheet database Model
    public class ServiceSheetVM
    {
        public ServiceSheetVM()
        {
            ServiceDaysVM = new List<ServiceDayVM>();
        }

        //Constructor for the service vm and day vm
        public ServiceSheetVM(ServiceSheet serviceSheetModel)
        {
            this.Id = serviceSheetModel.Id;
            this.AppName = serviceSheetModel.AppName;
            this.CanvasResponseId = serviceSheetModel.CanvasResponseId;
            this.SubmissionFormName = serviceSheetModel.SubmissionFormName;
            this.SubmissionFormVersion = serviceSheetModel.SubmissionFormVersion;
            this.DtDevice = serviceSheetModel.DtDevice;
            this.DtResponse = serviceSheetModel.DtResponse;
            this.SubmissionNumber = serviceSheetModel.SubmissionNumber;
            this.Username = serviceSheetModel.Username;
            this.UserFirstName = serviceSheetModel.UserFirstName;
            this.UserSurname = serviceSheetModel.UserSurname;
            this.Customer = serviceSheetModel.Customer;
            this.AddressLine1 = serviceSheetModel.AddressLine1;
            this.AddressLine2 = serviceSheetModel.AddressLine2;
            this.TownCity = serviceSheetModel.TownCity;
            this.Postcode = serviceSheetModel.Postcode;
            this.CustomerContact = serviceSheetModel.CustomerContact;
            this.CustomerPhoneNo = serviceSheetModel.CustomerPhoneNo;
            this.MachineMakeModel = serviceSheetModel.MachineMakeModel;
            this.MachineSerial = serviceSheetModel.MachineSerial;
            this.CncControl = serviceSheetModel.CncControl;
            this.CustomerOrderNo = serviceSheetModel.CustomerOrderNo;
            this.MttJobNumber = serviceSheetModel.MttJobNumber;
            this.JobDescription = serviceSheetModel.JobDescription;
            this.JobTotalTimeOnsite = serviceSheetModel.JobTotalTimeOnsite;
            this.JobTotalTravelTime = serviceSheetModel.JobTotalTravelTime;
            this.JobTotalMileage = serviceSheetModel.JobTotalMileage;
            this.TotalDailyAllowances = serviceSheetModel.TotalDailyAllowances;
            this.TotalOvernightAllowances = serviceSheetModel.TotalOvernightAllowances;
            this.TotalBarrierPayments = serviceSheetModel.TotalBarrierPayments;
            this.JobStatus = serviceSheetModel.JobStatus;
            this.FinalJobReport = serviceSheetModel.FinalJobReport;
            this.AdditionalFaults = serviceSheetModel.AdditionalFaults;
            this.QuoteRequired = serviceSheetModel.QuoteRequired;
            this.FollowUpPartsRequired = serviceSheetModel.FollowUpPartsRequired;
            this.Image1Url = serviceSheetModel.Image1Url;
            this.Image2Url = serviceSheetModel.Image2Url;
            this.Image3Url = serviceSheetModel.Image3Url;
            this.Image4Url = serviceSheetModel.Image4Url;
            this.Image5Url = serviceSheetModel.Image5Url;
            this.CustomerSignatureUrl = serviceSheetModel.CustomerSignatureUrl;
            this.CustomerName = serviceSheetModel.CustomerName;
            this.DtSigned = serviceSheetModel.DtSigned;
            this.MttEngSignatureUrl = serviceSheetModel.MttEngSignatureUrl;
            this.DtJobStart = serviceSheetModel.DtJobStart;

            this.ServiceDaysVM = new List<ServiceDayVM>();
            foreach (var day in serviceSheetModel.ServiceDays)
            {
                ServiceDayVM dayVM = new ServiceDayVM(day);
                this.ServiceDaysVM.Add(dayVM);
            }

            //RT 22/12/17 - Ordering the service days
            this.ServiceDaysVM.OrderBy(s => s.DtReport);

            //Default the include images on pdf to true
            IncludeImage1 = true;
            IncludeImage2 = true;
            IncludeImage3 = true;
            IncludeImage4 = true;
            IncludeImage5 = true;
            IncludeCustomerSignature = true;
        }

        public static ServiceSheet CreateServiceSheetFromVM(ServiceSheetApprovalVM vm)
        {
            //Creates a service sheet entity for saving to the database
            ServiceSheetVM sheetVM = vm.ServiceSheetModel;
            ServiceSheet retval = new ServiceSheet
            {
                AdditionalFaults = sheetVM.AdditionalFaults,
                AddressLine1 = sheetVM.AddressLine1,
                AddressLine2 = sheetVM.AddressLine2,
                AppName = sheetVM.AppName,
                CanvasResponseId = sheetVM.CanvasResponseId,
                CncControl = sheetVM.CncControl,
                Customer = sheetVM.Customer,
                CustomerContact = sheetVM.CustomerContact,
                CustomerName = sheetVM.CustomerName,
                CustomerOrderNo = sheetVM.CustomerOrderNo,
                CustomerPhoneNo = sheetVM.CustomerPhoneNo,
                CustomerSignatureUrl = sheetVM.CustomerSignatureUrl,
                DtDevice = sheetVM.DtDevice,
                DtJobStart = sheetVM.DtJobStart,
                DtResponse = sheetVM.DtResponse,
                DtSigned = sheetVM.DtSigned,
                FinalJobReport = sheetVM.FinalJobReport,
                FollowUpPartsRequired = sheetVM.FollowUpPartsRequired,
                Image1Url = sheetVM.Image1Url,
                Image2Url = sheetVM.Image2Url,
                Image3Url = sheetVM.Image3Url,
                Image4Url = sheetVM.Image4Url,
                Image5Url = sheetVM.Image5Url,
                JobDescription = sheetVM.JobDescription,
                JobStatus = sheetVM.JobStatus,
                JobTotalMileage = sheetVM.JobTotalMileage,
                JobTotalTimeOnsite = sheetVM.JobTotalTimeOnsite,
                JobTotalTravelTime = sheetVM.JobTotalTravelTime,
                MachineMakeModel = sheetVM.MachineMakeModel,
                MachineSerial = sheetVM.MachineSerial,
                MttEngSignatureUrl = sheetVM.MttEngSignatureUrl,
                MttJobNumber = sheetVM.MttJobNumber,
                Postcode = sheetVM.Postcode,
                QuoteRequired = sheetVM.QuoteRequired,
                SubmissionFormName = sheetVM.SubmissionFormName,
                SubmissionFormVersion = sheetVM.SubmissionFormVersion,
                SubmissionNumber = sheetVM.SubmissionNumber,
                TotalBarrierPayments = sheetVM.TotalBarrierPayments,
                TotalDailyAllowances = sheetVM.TotalDailyAllowances,
                TotalOvernightAllowances = sheetVM.TotalOvernightAllowances,
                TownCity = sheetVM.TownCity,
                UserFirstName = sheetVM.UserFirstName,
                Username = sheetVM.Username,
                UserSurname = sheetVM.UserSurname
            };

            return retval;
        }

        //Updates all the properties on the servicesheet which can be updated.  Also call update on service days
        public static void UpdateSheetAndDayModels(ServiceSheet updateSheet, ServiceSheetVM sheetVM)
        {
            updateSheet.AdditionalFaults = sheetVM.AdditionalFaults;
            updateSheet.AddressLine1 = sheetVM.AddressLine1;
            updateSheet.AddressLine2 = sheetVM.AddressLine2;
            updateSheet.CncControl = sheetVM.CncControl;
            updateSheet.Customer = sheetVM.Customer;
            updateSheet.CustomerContact = sheetVM.CustomerContact;
            updateSheet.CustomerName = sheetVM.CustomerName;
            updateSheet.CustomerOrderNo = sheetVM.CustomerOrderNo;
            updateSheet.CustomerPhoneNo = sheetVM.CustomerPhoneNo;
            updateSheet.DtJobStart = sheetVM.DtJobStart;
            updateSheet.DtSigned = sheetVM.DtSigned;
            updateSheet.FinalJobReport = sheetVM.FinalJobReport;
            updateSheet.FollowUpPartsRequired = sheetVM.FollowUpPartsRequired;
            updateSheet.JobDescription = sheetVM.JobDescription;
            updateSheet.JobStatus = sheetVM.JobStatus;
            updateSheet.JobTotalMileage = sheetVM.JobTotalMileage;
            updateSheet.JobTotalTimeOnsite = sheetVM.JobTotalTimeOnsite;
            updateSheet.JobTotalTravelTime = sheetVM.JobTotalTravelTime;
            updateSheet.MachineMakeModel = sheetVM.MachineMakeModel;
            updateSheet.MachineSerial = sheetVM.MachineSerial;
            updateSheet.MttJobNumber = sheetVM.MttJobNumber;
            updateSheet.Postcode = sheetVM.Postcode;
            updateSheet.QuoteRequired = sheetVM.QuoteRequired;
            updateSheet.TotalBarrierPayments = sheetVM.TotalBarrierPayments;
            updateSheet.TotalDailyAllowances = sheetVM.TotalDailyAllowances;
            updateSheet.TotalOvernightAllowances = sheetVM.TotalOvernightAllowances;
            updateSheet.TownCity = sheetVM.TownCity;

            //Update the service days
            foreach (var dayModel in updateSheet.ServiceDays)
            {
                int dayId = dayModel.Id;
                ServiceDayVM dayVM = sheetVM.ServiceDaysVM.Where(d => d.Id == dayId).FirstOrDefault();
                if (dayVM == null)
                {
                    throw new Exception("Day VM not found for id: " + dayId);
                }
                dayVM.UpdateDay(dayModel);
            }
        }

        [Editable(false)]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Submission Number")]
        [Editable(false)]
        [Required]
        public int SubmissionNumber { get; set; }

        //Internal use only - not displayed anywhere
        [Required]
        public string AppName { get; set; }

        [Editable(false)]
        [Required]
        public string Username { get; set; }

        [Editable(false)]
        [Display(Name = "Engineer First Name")]
        public string UserFirstName { get; set; }

        [Editable(false)]
        [Display(Name = "Engineer Surname")]
        [Required]
        public string UserSurname { get; set; }

        [Required]
        //Internal use only - not displayed anywhere
        public string CanvasResponseId { get; set; }

        [Required]
        //Internal use only - not displayed anywhere
        public System.DateTime DtResponse { get; set; }

        [Required]
        //Internal use only - not displayed anywhere
        public System.DateTime DtDevice { get; set; }

        [Required]
        //Internal use only - not displayed anywhere
        public string SubmissionFormName { get; set; }

        [Required]
        //Internal use only - not displayed anywhere
        public int SubmissionFormVersion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Customer is required.")]
        [StringLength(255, ErrorMessage = "Maximum customer length is 255")]
        public string Customer { get; set; }

        [StringLength(255, ErrorMessage = "Maximum address length is 255")]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [StringLength(255, ErrorMessage = "Maximum address length is 255")]
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [StringLength(100, ErrorMessage = "Maximum length is 100")]
        [Display(Name = "Town/City")]
        public string TownCity { get; set; }

        [StringLength(10, ErrorMessage = "Maximum length is 10")]
        public string Postcode { get; set; }

        [StringLength(100, ErrorMessage = "Maximum length is 100")]
        [Display(Name = "Customer Contact")]
        public string CustomerContact { get; set; }

        [StringLength(20, ErrorMessage = "Maximum length is 20")]
        [Display(Name = "Customer Phone No")]
        public string CustomerPhoneNo { get; set; }

        [StringLength(100, ErrorMessage = "Maximum length is 100")]
        [Display(Name = "Machine Make/Model")]
        public string MachineMakeModel { get; set; }

        [StringLength(100, ErrorMessage = "Maximum length is 100")]
        [Display(Name = "Machine Serial No")]
        public string MachineSerial { get; set; }

        [StringLength(100, ErrorMessage = "Maximum length is 100")]
        [Display(Name = "CNC Control")]
        public string CncControl { get; set; }

        [Required(ErrorMessage = "Job start date is required")]
        [Display(Name = "Job Start Date")]
        [DataType(DataType.Date)]
        public System.DateTime DtJobStart { get; set; }

        [StringLength(100, ErrorMessage = "Maximum length is 100")]
        [Display(Name = "Customer Order No")]
        public string CustomerOrderNo { get; set; }

        [Required(ErrorMessage = "MTT job number is required")]
        [StringLength(20, ErrorMessage = "Maximum length is 20")]
        [Display(Name = "MTT Job Number")]
        public string MttJobNumber { get; set; }

        [Required(ErrorMessage = "Job description is required")]
        [Display(Name = "Job Description")]
        public string JobDescription { get; set; }

        [Required(ErrorMessage = "Total time onsite is required")]
        [Display(Name = "Total Time Onsite")]
        public double JobTotalTimeOnsite { get; set; }

        [Required(ErrorMessage = "Total travel time is required")]
        [Display(Name = "Total Travel Time")]
        public double JobTotalTravelTime { get; set; }

        [Required(ErrorMessage = "Total mileage is required")]
        [Display(Name = "Total Mileage")]
        public int JobTotalMileage { get; set; }
        
        [Required(ErrorMessage = "Total daily allowances are required")]
        [Display(Name = "Total Daily Allowances")]
        public int TotalDailyAllowances { get; set; }

        [Required(ErrorMessage = "Total overnight allowances are required")]
        [Display(Name = "Total Overnight Allowances")]
        public int TotalOvernightAllowances { get; set; }

        [Required(ErrorMessage = "Total barrier payments are required")]
        [Display(Name = "Total Barrier Payments")]
        public int TotalBarrierPayments { get; set; }

        [Required(ErrorMessage = "Job status is required")]
        [StringLength(100, ErrorMessage = "Maximum length is 100")]
        [Display(Name = "Job Status")]
        public string JobStatus { get; set; }

        [Required(ErrorMessage = "Final job report is required")]
        [Display(Name = "Final Job Report")]
        public string FinalJobReport { get; set; }

        [Display(Name = "Additional Faults Found")]
        public string AdditionalFaults { get; set; }

        [Required]
        [Display(Name = "Quote Required")]
        public bool QuoteRequired { get; set; }

        [Display(Name = "Follow-up Parts Required")]
        public string FollowUpPartsRequired { get; set; }

        [StringLength(255, ErrorMessage = "Maximum length is 255")]
        [Editable(false)]
        public string Image1Url { get; set; }

        [StringLength(255, ErrorMessage = "Maximum length is 255")]
        [Editable(false)]
        public string Image2Url { get; set; }

        [StringLength(255, ErrorMessage = "Maximum length is 255")]
        [Editable(false)]
        public string Image3Url { get; set; }

        [StringLength(255, ErrorMessage = "Maximum length is 255")]
        [Editable(false)]
        public string Image4Url { get; set; }

        [StringLength(255, ErrorMessage = "Maximum length is 255")]
        [Editable(false)]
        public string Image5Url { get; set; }


        public string CustomerSignatureUrl { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(255, ErrorMessage = "Maximum length is 255")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Date signed is required")]
        [Display(Name = "Date Signed")]
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime DtSigned { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Maximum length is 255")]
        [Editable(false)]
        public string MttEngSignatureUrl { get; set; }

        //Non db properties
        //Options to include the images
        [Display(Name = "Display Image 1")]
        public bool IncludeImage1 { get; set; }
        [Display(Name = "Display Image 2")]
        public bool IncludeImage2 { get; set; }
        [Display(Name = "Display Image 3")]
        public bool IncludeImage3 { get; set; }
        [Display(Name = "Display Image 4")]
        public bool IncludeImage4 { get; set; }
        [Display(Name = "Display Image 5")]
        public bool IncludeImage5 { get; set; }
        [Display(Name = "Display Customer Signature")]
        public bool IncludeCustomerSignature { get; set; }

        [Display(Name = "Image 1")]
        public string Image1Base64String { get; set; }
        [Display(Name = "Image 2")]
        public string Image2Base64String { get; set; }
        [Display(Name = "Image 3")]
        public string Image3Base64String { get; set; }
        [Display(Name = "Image 4")]
        public string Image4Base64String { get; set; }
        [Display(Name = "Image 5")]
        public string Image5Base64String { get; set; }
        [Display(Name = "Customer Signature")]
        public string ImageCustomerSignatureBase64String { get; set; }
        [Display(Name = "Engineer Signature")]
        public string ImageEngineerSignatureBase64String { get; set; }

        //Utility methods
        [Display(Name = "Engineer")]
        public string EngineerFullName { get { return UserFirstName + " " + UserSurname; } }


        public void LoadCanvasImages()
        {
            //These methods make calls to the canvas api
            Image1Base64String = CanvasImageHelpers.LoadCanvasImageForUrl(Image1Url);
            Image2Base64String = CanvasImageHelpers.LoadCanvasImageForUrl(Image2Url);
            Image3Base64String = CanvasImageHelpers.LoadCanvasImageForUrl(Image3Url);
            Image4Base64String = CanvasImageHelpers.LoadCanvasImageForUrl(Image4Url);
            Image5Base64String = CanvasImageHelpers.LoadCanvasImageForUrl(Image5Url);

            ImageEngineerSignatureBase64String = CanvasImageHelpers.LoadCanvasImageForUrl(MttEngSignatureUrl);
            ImageCustomerSignatureBase64String = CanvasImageHelpers.LoadCanvasImageForUrl(CustomerSignatureUrl);
        }

        public List<ServiceDayVM> ServiceDaysVM { get; set; }
        
    }
}