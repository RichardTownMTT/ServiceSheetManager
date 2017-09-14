using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;
using ServiceSheetManager.Models;

namespace ServiceSheetManager.ViewModels
{
    //View Model for the service sheet ListReports View
    public class ServiceSheetListVM
    {
        public const string customerSortAsc = "customerSortAsc";
        public const string customerSortDesc = "customerSortDesc";
        public const string submissionSortAsc = "submissionSortAsc";
        public const string submissionSortDesc = "submissionSortDesc";
        public const string jobNumberSortAsc = "jobNumberSortAsc";
        public const string jobNumberSortDesc = "jobNumberSortDesc";
        public const string machineSortAsc = "machineSortAsc";
        public const string machineSortDesc = "machineSortDesc";
        public const string engineerSortAsc = "engineerSortAsc";
        public const string engineerSortDesc = "engineerSortDesc";

        public IPagedList<ServiceSheetVM> ServiceSheets { get; set; }
        
        public string SortOrder { get; set; }
        public string CurrentSortOrder { get; set; }

        public string SwitchCustomerSort {
            get {
                return CurrentSortOrder.Equals(customerSortAsc) ? customerSortDesc : customerSortAsc;
            } }

        public string SwitchSubmissionSort
        {
            get
            {
                return CurrentSortOrder.Equals(submissionSortAsc) ? submissionSortDesc : submissionSortAsc;
            }
        }

        public string SwitchJobNumberSort
        {
            get
            {
                return CurrentSortOrder.Equals(jobNumberSortAsc) ? jobNumberSortDesc : jobNumberSortAsc;
            }
        }

        public string SwitchMachineSort
        {
            get
            {
                return CurrentSortOrder.Equals(machineSortAsc) ? machineSortDesc : machineSortAsc;
            }
        }

        public string SwitchEngineerSort
        {
            get
            {
                return CurrentSortOrder.Equals(engineerSortAsc) ? engineerSortDesc : engineerSortAsc;
            }
        }


        public ServiceSheetListVM ()
        {
            ServiceSheets = new PagedList<ServiceSheetVM>(new List<ServiceSheetVM>(), 1, 1);
        }
    }
}