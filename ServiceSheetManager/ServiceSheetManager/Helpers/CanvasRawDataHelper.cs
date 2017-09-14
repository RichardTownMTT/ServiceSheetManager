using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSheetManager.Helpers
{
    public class CanvasRawDataHelper
    {
        public static CanvasRawData CopyEntity(CanvasRawData template)
        {
            CanvasRawData retval = new CanvasRawData();
            retval.AdditionalFaults = template.AdditionalFaults;
            retval.AddressLine1 = template.AddressLine1;
            retval.AddressLine2 = template.AddressLine2;
            retval.AppName = template.AppName;
            retval.Approved = template.Approved;
            retval.CanvasResponseId = template.CanvasResponseId;
            retval.CncControl = template.CncControl;
            retval.Customer = template.Customer;
            retval.CustomerContact = template.CustomerContact;
            retval.CustomerName = template.CustomerName;
            retval.CustomerOrderNo = template.CustomerOrderNo;
            retval.CustomerPhoneNo = template.CustomerPhoneNo;
            retval.CustomerSignatureUrl = template.CustomerSignatureUrl;
            retval.DtDevice = template.DtDevice;
            retval.DtEndSubmission = template.DtEndSubmission;
            retval.DtJobStart = template.DtJobStart;
            retval.DtResponse = template.DtResponse;
            retval.DtSigned = template.DtSigned;
            retval.DtStartSubmission = template.DtStartSubmission;
            retval.FinalJobReport = template.FinalJobReport;
            retval.FollowUpPartsRequired = template.FollowUpPartsRequired;
            retval.Image1Url = template.Image1Url;
            retval.Image2Url = template.Image2Url;
            retval.Image3Url = template.Image3Url;
            retval.Image4Url = template.Image4Url;
            retval.Image5Url = template.Image5Url;
            retval.JobDescription = template.JobDescription;
            retval.JobStatus = template.JobStatus;
            retval.JobTotalMileage = template.JobTotalMileage;
            retval.JobTotalTimeOnsite = template.JobTotalTimeOnsite;
            retval.JobTotalTravelTime = template.JobTotalTravelTime;
            retval.MachineMakeModel = template.MachineMakeModel;
            retval.MachineSerial = template.MachineSerial;
            retval.MttEngSignatureUrl = template.MttEngSignatureUrl;
            retval.MttJobNumber = template.MttJobNumber;
            retval.Postcode = template.Postcode;
            retval.QuoteRequired = template.QuoteRequired;
            retval.SubmissionFormName = template.SubmissionFormName;
            retval.SubmissionFormVersion = template.SubmissionFormVersion;
            retval.SubmissionNumber = template.SubmissionNumber;
            retval.TotalBarrierPayments = template.TotalBarrierPayments;
            retval.TotalDailyAllowances = template.TotalDailyAllowances;
            retval.TotalOvernightAllowances = template.TotalOvernightAllowances;
            retval.TownCity = template.TownCity;
            retval.UserFirstName = template.UserFirstName;
            retval.Username = template.Username;
            retval.UserSurname = template.UserSurname;

            return retval;
        }

        public static ServiceSheetApprovalVM CreateServiceEntitiesForCanvasEntities(List<CanvasRawData> canvasEntities)
        {
            ServiceSheetApprovalVM approvalVM = new ServiceSheetApprovalVM();

            CanvasRawData firstCanvasEntity = canvasEntities.First();

            ServiceSheet serviceSheetCreated = CreateServiceSheetForCanvasEntity(firstCanvasEntity);
            approvalVM.ServiceSheetModel = serviceSheetCreated;

            List<ServiceDayCreateVM> createServiceDayVMs = new List<ServiceDayCreateVM>();

            //Temp id counter used to create id's for the service days.  These aren't saved to the db yet
            int tempIdCounter = 1;
            foreach (var canvasEntity in canvasEntities)
            {
                ServiceDay day = CreateServiceDayForCanvasEntity(canvasEntity, serviceSheetCreated);

                ServiceDayCreateVM vm = new ServiceDayCreateVM();
                vm.ServiceDayEntity = day;
                vm.TempId = tempIdCounter;
                createServiceDayVMs.Add(vm);
                
                tempIdCounter++;
            }
            approvalVM.ServiceDayModels = createServiceDayVMs;

            return approvalVM;
        }

        private static ServiceDay CreateServiceDayForCanvasEntity(CanvasRawData template, ServiceSheet serviceSheetCreated)
        {
            ServiceDay retval = new ServiceDay();
            retval.ArrivalOnsiteTime = template.ArrivalOnsiteTime;
            retval.BarrierPayment = template.BarrierPayment;
            retval.DailyAllowance = template.DailyAllowance;
            retval.DailyReport = template.DailyReport;
            retval.DepartureSiteTime = template.DepartureSiteTime;
            retval.DtReport = template.DtReport;
            retval.Mileage = template.Mileage;
            retval.OvernightAllowance = template.OvernightAllowance;
            retval.PartsSuppliedToday = template.PartsSuppliedToday;
            retval.TotalOnsiteTime = template.TotalOnsiteTime;
            retval.TotalTravelTime = template.TotalTravelTime;
            retval.TravelEndTime = template.TravelEndTime;
            retval.TravelFromSiteTime = template.TravelFromSiteTime;
            retval.TravelStartTime = template.TravelStartTime;
            retval.TravelToSiteTime = template.TravelToSiteTime;

            return retval;
        }

        private static ServiceSheet CreateServiceSheetForCanvasEntity(CanvasRawData template)
        {
            ServiceSheet retval = new ServiceSheet();
            retval.AdditionalFaults = template.AdditionalFaults;
            retval.AddressLine1 = template.AddressLine1;
            retval.AddressLine2 = template.AddressLine2;
            retval.AppName = template.AppName;
            retval.CanvasResponseId = template.CanvasResponseId;
            retval.CncControl = template.CncControl;
            retval.Customer = template.Customer;
            retval.CustomerContact = template.CustomerContact;
            retval.CustomerName = template.CustomerName;
            retval.CustomerOrderNo = template.CustomerOrderNo;
            retval.CustomerPhoneNo = template.CustomerPhoneNo;
            retval.CustomerSignatureUrl = template.CustomerSignatureUrl;
            retval.DtDevice = template.DtDevice;
            retval.DtJobStart = template.DtJobStart;
            retval.DtResponse = template.DtResponse;
            retval.DtSigned = template.DtSigned;
            retval.FinalJobReport = template.FinalJobReport;
            retval.FollowUpPartsRequired = template.FollowUpPartsRequired;
            retval.Image1Url = template.Image1Url;
            retval.Image2Url = template.Image2Url;
            retval.Image3Url = template.Image3Url;
            retval.Image4Url = template.Image4Url;
            retval.Image5Url = template.Image5Url;
            retval.JobDescription = template.JobDescription;
            retval.JobStatus = template.JobStatus;
            retval.JobTotalMileage = template.JobTotalMileage;
            retval.JobTotalTimeOnsite = template.JobTotalTimeOnsite;
            retval.JobTotalTravelTime = template.JobTotalTravelTime;
            retval.MachineMakeModel = template.MachineMakeModel;
            retval.MachineSerial = template.MachineSerial;
            retval.MttEngSignatureUrl = template.MttEngSignatureUrl;
            retval.MttJobNumber = template.MttJobNumber;
            retval.Postcode = template.Postcode;
            retval.QuoteRequired = template.QuoteRequired;
            retval.SubmissionFormName = template.SubmissionFormName;
            retval.SubmissionFormVersion = template.SubmissionFormVersion;
            retval.SubmissionNumber = template.SubmissionNumber;
            retval.TotalBarrierPayments = template.TotalBarrierPayments;
            retval.TotalDailyAllowances = template.TotalDailyAllowances;
            retval.TotalOvernightAllowances = template.TotalOvernightAllowances;
            retval.TownCity = template.TownCity;
            retval.UserFirstName = template.UserFirstName;
            retval.Username = template.Username;
            retval.UserSurname = template.UserSurname;

            return retval;
        }
    }
}