using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels
{
    public class ServiceSheetApprovalIndexVM
    {
        public int Id { get; set; }

        [Display(Name = "Submission No")]
        public int SubmissionNumber { get; set; }
        public string Username { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Customer { get; set; }
        [Display(Name ="Machine Make")]
        public string MachineMake { get; set; }
        [Display(Name = "Serial No")]
        public string MachineSerial { get; set; }
        [Display(Name = "Job Start Date")]
        [DataType(DataType.Date)]
        public DateTime JobStart { get; set; }
        [Display(Name = "Job Number")]
        public string MttJobNumber { get; set; }
        public bool Approved { get; set; }
    }
}