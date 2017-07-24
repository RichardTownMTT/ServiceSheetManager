using ServiceSheetManager.Models;
using ServiceSheetManager.Models.NonDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.ViewModels
{
    public class MatrixServiceStatsVM
    {
        public List<JobNumberItem> AllJobNumbers { get; set; }
        public MatrixServiceOverview ServiceOverview { get; set; }

        public MatrixServiceStatsVM()
        {
            AllJobNumbers = new List<JobNumberItem>();
        }
    }
}