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
            CanvasRawData retval = new CanvasRawData
            {
                AdditionalFaults = template.AdditionalFaults,
                AddressLine1 = template.AddressLine1,
                AddressLine2 = template.AddressLine2,
                AppName = template.AppName,
                Approved = template.Approved,
                CanvasResponseId = template.CanvasResponseId,
                CncControl = template.CncControl,
                Customer = template.Customer,
                CustomerContact = template.CustomerContact,
                CustomerName = template.CustomerName,
                CustomerOrderNo = template.CustomerOrderNo,
                CustomerPhoneNo = template.CustomerPhoneNo,
                CustomerSignatureUrl = template.CustomerSignatureUrl,
                DtDevice = template.DtDevice,
                DtEndSubmission = template.DtEndSubmission,
                DtJobStart = template.DtJobStart,
                DtResponse = template.DtResponse,
                DtSigned = template.DtSigned,
                DtStartSubmission = template.DtStartSubmission,
                FinalJobReport = template.FinalJobReport,
                FollowUpPartsRequired = template.FollowUpPartsRequired,
                Image1Url = template.Image1Url,
                Image2Url = template.Image2Url,
                Image3Url = template.Image3Url,
                Image4Url = template.Image4Url,
                Image5Url = template.Image5Url,
                JobDescription = template.JobDescription,
                JobStatus = template.JobStatus,
                JobTotalMileage = template.JobTotalMileage,
                JobTotalTimeOnsite = template.JobTotalTimeOnsite,
                JobTotalTravelTime = template.JobTotalTravelTime,
                MachineMakeModel = template.MachineMakeModel,
                MachineSerial = template.MachineSerial,
                MttEngSignatureUrl = template.MttEngSignatureUrl,
                MttJobNumber = template.MttJobNumber,
                Postcode = template.Postcode,
                QuoteRequired = template.QuoteRequired,
                SubmissionFormName = template.SubmissionFormName,
                SubmissionFormVersion = template.SubmissionFormVersion,
                SubmissionNumber = template.SubmissionNumber,
                TotalBarrierPayments = template.TotalBarrierPayments,
                TotalDailyAllowances = template.TotalDailyAllowances,
                TotalOvernightAllowances = template.TotalOvernightAllowances,
                TownCity = template.TownCity,
                UserFirstName = template.UserFirstName,
                Username = template.Username,
                UserSurname = template.UserSurname
            };

            return retval;
        }

        public static ServiceSheetApprovalVM CreateServiceEntitiesForCanvasEntities(List<CanvasRawData> canvasEntities)
        {
            ServiceSheetApprovalVM approvalVM = new ServiceSheetApprovalVM();

            CanvasRawData firstCanvasEntity = canvasEntities.First();

            ServiceSheet serviceSheetCreated = CreateServiceSheetForCanvasEntity(firstCanvasEntity);
            ServiceSheetVM sheetVM = new ServiceSheetVM(serviceSheetCreated);
            approvalVM.ServiceSheetModel = sheetVM;

            List<ServiceDayCreateVM> createServiceDayVMs = new List<ServiceDayCreateVM>();

            //Temp id counter used to create id's for the service days.  These aren't saved to the db yet
            int tempIdCounter = 1;
            foreach (var canvasEntity in canvasEntities)
            {
                ServiceDay day = CreateServiceDayForCanvasEntity(canvasEntity, serviceSheetCreated);

                ServiceDayCreateVM vm = new ServiceDayCreateVM
                {
                    //vm.ServiceDayEntity = day;
                    ServiceDayEntity = new ServiceDayVM(day),
                    TempId = tempIdCounter
                };
                createServiceDayVMs.Add(vm);
                
                tempIdCounter++;
            }
            approvalVM.ServiceDayModels = createServiceDayVMs;

            return approvalVM;
        }

        private static ServiceDay CreateServiceDayForCanvasEntity(CanvasRawData template, ServiceSheet serviceSheetCreated)
        {
            ServiceDay retval = new ServiceDay
            {
                ArrivalOnsiteTime = template.ArrivalOnsiteTime,
                BarrierPayment = template.BarrierPayment,
                DailyAllowance = template.DailyAllowance,
                DailyReport = template.DailyReport,
                DepartureSiteTime = template.DepartureSiteTime,
                DtReport = template.DtReport,
                Mileage = template.Mileage,
                OvernightAllowance = template.OvernightAllowance,
                PartsSuppliedToday = template.PartsSuppliedToday,
                TotalOnsiteTime = template.TotalOnsiteTime,
                TotalTravelTime = template.TotalTravelTime,
                TravelEndTime = template.TravelEndTime,
                TravelFromSiteTime = template.TravelFromSiteTime,
                TravelStartTime = template.TravelStartTime,
                TravelToSiteTime = template.TravelToSiteTime
            };

            return retval;
        }

        private static ServiceSheet CreateServiceSheetForCanvasEntity(CanvasRawData template)
        {
            ServiceSheet retval = new ServiceSheet
            {
                AdditionalFaults = template.AdditionalFaults,
                AddressLine1 = template.AddressLine1,
                AddressLine2 = template.AddressLine2,
                AppName = template.AppName,
                CanvasResponseId = template.CanvasResponseId,
                CncControl = template.CncControl,
                Customer = template.Customer,
                CustomerContact = template.CustomerContact,
                CustomerName = template.CustomerName,
                CustomerOrderNo = template.CustomerOrderNo,
                CustomerPhoneNo = template.CustomerPhoneNo,
                CustomerSignatureUrl = template.CustomerSignatureUrl,
                DtDevice = template.DtDevice,
                DtJobStart = template.DtJobStart,
                DtResponse = template.DtResponse,
                DtSigned = template.DtSigned,
                FinalJobReport = template.FinalJobReport,
                FollowUpPartsRequired = template.FollowUpPartsRequired,
                Image1Url = template.Image1Url,
                Image2Url = template.Image2Url,
                Image3Url = template.Image3Url,
                Image4Url = template.Image4Url,
                Image5Url = template.Image5Url,
                JobDescription = template.JobDescription,
                JobStatus = template.JobStatus,
                JobTotalMileage = template.JobTotalMileage,
                JobTotalTimeOnsite = template.JobTotalTimeOnsite,
                JobTotalTravelTime = template.JobTotalTravelTime,
                MachineMakeModel = template.MachineMakeModel,
                MachineSerial = template.MachineSerial,
                MttEngSignatureUrl = template.MttEngSignatureUrl,
                MttJobNumber = template.MttJobNumber,
                Postcode = template.Postcode,
                QuoteRequired = template.QuoteRequired,
                SubmissionFormName = template.SubmissionFormName,
                SubmissionFormVersion = template.SubmissionFormVersion,
                SubmissionNumber = template.SubmissionNumber,
                TotalBarrierPayments = template.TotalBarrierPayments,
                TotalDailyAllowances = template.TotalDailyAllowances,
                TotalOvernightAllowances = template.TotalOvernightAllowances,
                TownCity = template.TownCity,
                UserFirstName = template.UserFirstName,
                Username = template.Username,
                UserSurname = template.UserSurname
            };

            return retval;
        }
    }
}