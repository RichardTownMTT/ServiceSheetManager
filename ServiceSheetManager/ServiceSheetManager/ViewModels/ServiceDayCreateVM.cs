using ServiceSheetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels
{
    public class ServiceDayCreateVM
    {
        //Used for creating service days
        public ServiceDayVM ServiceDayEntity { get; set; }
        //Temp id used for view screens where the id has not been set as we haven't created the entity yet
        public int TempId { get; set; }
    }
}