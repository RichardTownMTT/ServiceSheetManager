using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceSheetManager.ViewModels
{
    public class ServiceSheetIndexVM
    {
        public List<ServiceSheet> ServiceSheets { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerSearch { get; set; }

        [Display(Name = "MTT Job Number")]
        public string MttJobNumberSearch { get; set; }

        [Display(Name = "From")]
        [DataType(DataType.Date)]
        public DateTime SheetsFromDateSearch { get; set; }

        [Display(Name = "To")]
        [DataType(DataType.Date)]
        public DateTime SheetsToDateSearch { get; set; }

        [Display(Name = "From")]
        [DataType(DataType.Date)]
        public DateTime SheetsFromEngineerSearch { get; set; }

        [Display(Name = "To")]
        [DataType(DataType.Date)]
        public DateTime SheetsToEngineerSearch { get; set; }

        [Display(Name = "Engineer")]
        public List<SelectListItem> Engineers { get; set; }
        public string SelectedEngineer { get; set; }

        [Display(Name = "Submission Number")]
        public int SubmissionNumber { get; set; }

        public ServiceSheetIndexVM()
        {
            ServiceSheets = new List<ServiceSheet>();
        }
    }
}