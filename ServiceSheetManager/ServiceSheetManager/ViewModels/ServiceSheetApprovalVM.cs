using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceSheetManager.ViewModels
{
    public class ServiceSheetApprovalVM
    {
        public ServiceSheetApprovalVM()
        {
            ServiceDayModels = new List<ServiceDayCreateVM>();
            Errors = false;
        }

        //This returns one service sheet and multiple service days for viewing / editing
        public ServiceSheetVM ServiceSheetModel { get; set; }
        public List<ServiceDayCreateVM> ServiceDayModels { get; set; }
        public bool Errors { get; set; }
    }
}