using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PagedList;
using System.Web.Mvc;

namespace ServiceSheetManager.ViewModels
{
    //ViewModel for the Service Sheet index page, storing all the search terms
    public class ServiceSheetIndexVM
    {
        [Display(Name = "Customer Name")]
        public string CustomerSearch { get; set; }

        [Display(Name = "MTT Job Number")]
        public string MttJobNumberSearch { get; set; }

        [Display(Name = "Reports From")]
        [DataType(DataType.Date)]
        public DateTime? SheetsFromDateSearch { get; set; }

        [Display(Name = "Reports To")]
        [DataType(DataType.Date)]
        public DateTime? SheetsToDateSearch { get; set; }

        [Display(Name = "Engineer")]
        public List<SelectListItem> Engineers { get; set; }
        public string SelectedEngineer { get; set; }

        [Display(Name = "Submission Number")]
        public int? SubmissionNumber { get; set; }
        
    }
}