using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.Models
{
    [MetadataType(typeof(ServiceSheet.ServiceSheetMetaData))]
    public partial class ServiceSheet
    {
        internal sealed class ServiceSheetMetaData
        {
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
            public string SubmissionFormName { get; set; }

            [Required]
            //Internal use only - not displayed anywhere
            public int SubmissionFormVersion { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "Customer is required.")]
            [StringLength(255, ErrorMessage ="Maximum customer length is 255")]
            public string Customer { get; set; }

            [StringLength(255, ErrorMessage = "Maximum address length is 255")]
            [Display(Name = "Address Line 1")]
            public string AddressLine1 { get; set;}

            [StringLength(255, ErrorMessage = "Maximum address length is 255")]
            [Display(Name = "Address Line 2")]
            public string AddressLine2 { get; set; }

            [StringLength(100, ErrorMessage = "Maximum length is 100")]
            [Display(Name = "Town/City")]
            public string TownCity { get; set; }

            [StringLength(10, ErrorMessage = "Maximum length is 10")]
            public string Postcode { get; set; }

            [StringLength(100, ErrorMessage = "Maximum length is 100")]
            [Display(Name ="Customer Contact")]
            public string CustomerContact { get; set; }

            [StringLength(20, ErrorMessage = "Maximum length is 20")]
            [Display(Name ="Customer Phone No")]
            public string CustomerPhoneNo { get; set; }

            [StringLength(100, ErrorMessage = "Maximum length is 100")]
            [Display(Name ="Machine Make/Model")]
            public string MachineMakeModel { get; set; }

            [StringLength(100, ErrorMessage = "Maximum length is 100")]
            [Display(Name ="Machine Serial No")]
            public string MachineSerial { get; set; }

            [StringLength(100, ErrorMessage = "Maximum length is 100")]
            [Display(Name ="CNC Control")]
            public string CncControl { get; set; }

            [Required(ErrorMessage ="Job start date is required")]
            [Display(Name ="Job Start Date")]
            [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public System.DateTime DtJobStart { get; set; }

            [StringLength(100, ErrorMessage = "Maximum length is 100")]
            [Display(Name ="Customer Order No")]
            public string CustomerOrderNo { get; set; }

            [Required(ErrorMessage ="MTT job number is required")]
            [StringLength(20, ErrorMessage = "Maximum length is 20")]
            [Display(Name ="MTT Job Number")]
            public string MttJobNumber { get; set; }

            [Required(ErrorMessage ="Job description is required")]
            [Display(Name ="Job Description")]
            public string JobDescription { get; set; }

            [Required(ErrorMessage ="Total time onsite is required")]
            [Display(Name ="Total Time Onsite")]
            public double JobTotalTimeOnsite { get; set; }

            [Required(ErrorMessage ="Total travel time is required")]
            [Display(Name ="Total Travel Time")]
            public double JobTotalTravelTime { get; set; }

            [Required(ErrorMessage = "Total mileage is required")]
            [Display(Name ="Total Mileage")]
            public int JobTotalMileage { get; set; }

            [Required(ErrorMessage = "Total daily allowances are required")]
            [Display(Name ="Total Daily Allowances")]
            public int TotalDailyAllowances { get; set; }

            [Required(ErrorMessage = "Total overnight allowances are required")]
            [Display(Name ="Total Overnight Allowances")]
            public int TotalOvernightAllowances { get; set; }

            [Required(ErrorMessage = "Total barrier payments are required")]
            [Display(Name ="Total Barrier Payments")]
            public int TotalBarrierPayments { get; set; }

            [Required(ErrorMessage = "Job status is required")]
            [StringLength(100, ErrorMessage = "Maximum length is 100")]
            [Display(Name ="Job Status")]
            public string JobStatus { get; set; }

            [Required(ErrorMessage = "Final job report is required")]
            [Display(Name ="Final Job Report")]
            public string FinalJobReport { get; set; }

            [Display(Name ="Additional Faults Found")]
            public string AdditionalFaults { get; set; }

            [Required]
            [Display(Name ="Quote Required")]
            public bool QuoteRequired { get; set; }

            [Display(Name ="Follow-up Parts Required")]
            public string FollowUpPartsRequired { get; set; }

            [StringLength(255, ErrorMessage = "Maximum length is 255")]
            public string Image1Url { get; set; }

            [StringLength(255, ErrorMessage = "Maximum length is 255")]
            public string Image2Url { get; set; }

            [StringLength(255, ErrorMessage = "Maximum length is 255")]
            public string Image3Url { get; set; }

            [StringLength(255, ErrorMessage = "Maximum length is 255")]
            public string Image4Url { get; set; }

            [StringLength(255, ErrorMessage = "Maximum length is 255")]
            public string Image5Url { get; set; }


            public string CustomerSignatureUrl { get; set; }

            [Required(ErrorMessage ="Customer name is required")]
            [StringLength(255, ErrorMessage = "Maximum length is 255")]
            [Display(Name ="Customer Name")]
            public string CustomerName { get; set; }

            [Required(ErrorMessage = "Date signed is required")]
            [Display(Name ="Date Signed")]
            [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public System.DateTime DtSigned { get; set; }

            [Required]
            [StringLength(255, ErrorMessage = "Maximum length is 255")]
            public string MttEngSignatureUrl { get; set; }
        }
        
    }
}