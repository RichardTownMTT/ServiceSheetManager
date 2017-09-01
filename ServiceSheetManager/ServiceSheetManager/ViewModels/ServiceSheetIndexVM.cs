using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels
{
    public class ServiceSheetIndexVM
    {
        public List<ServiceSheet> ServiceSheets { get; set; }

        public ServiceSheetIndexVM()
        {
            ServiceSheets = new List<ServiceSheet>();
        }
    }
}