using ServiceSheetManager.Models;
using ServiceSheetManager.Helpers;
using ServiceSheetManager.Models.NonDbModels;
using ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Globalization;

namespace ServiceSheetManager.Controllers
{
    public class CanvasSubmissionController : Controller
    {
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        [HttpPost]
        public HttpResponseMessage AddServiceSubmission()
        {
            HttpResponseMessage httpResponse;
            //Load XML Stream

            XDocument xmlInput = null;
            if (HttpContext.IsDebuggingEnabled)
            {
                xmlInput = GetXmlStreamFromFile();
            }
            else
            {
                xmlInput = GetXmlStream();
            }

            if (xmlInput == null)
            {
                httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                return httpResponse;
            }

            CanvasApiSubmission apiSubmission = DeserialiseApiSubmission(xmlInput);
            System.Diagnostics.Trace.TraceError("Guid = " + apiSubmission.guid);

            XDocument canvasDataXml = DownloadXmlForGuid(apiSubmission.guid);

            if (canvasDataXml == null)
            {
                System.Diagnostics.Trace.TraceError("Error occured downloading submission for guid: " + apiSubmission.guid);
                httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                return httpResponse;
            }

            //Serialise to Canvas Raw Data Entities
            List<CanvasRawXmlItem> rawCanvasEntities = DeserialiseCanvasData(canvasDataXml);
            if (rawCanvasEntities == null)
            {
                //Log error and XML
                System.Diagnostics.Trace.TraceError("Error occured serialising data");
                httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                return httpResponse;
            }

            System.Diagnostics.Trace.TraceError("Creating entities");

            //Create CanvasRawData Entities 
            List<CanvasRawData> canvasEntities = new List<CanvasRawData>();
            foreach (var entitiyFromXml in rawCanvasEntities)
            {
                List<CanvasRawData> item = ConvertCanvasXmlEntityToDbEntity(entitiyFromXml);
                canvasEntities.AddRange(item);
                System.Diagnostics.Trace.TraceError("Number of entities: " + canvasEntities.Count);
            }

            System.Diagnostics.Trace.TraceError("Validating entities");
            //Validate Entities
            ValidateEntities(canvasEntities);

            System.Diagnostics.Trace.TraceError("Saving entities");
            //Save to database
            try
            {
                db.CanvasRawDatas.AddRange(canvasEntities);
                db.SaveChanges();
                System.Diagnostics.Trace.TraceError("Saved");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error occured while saving from canvas: " + ex.ToString());
            }

            //Return appropriate response code

            httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            return httpResponse;
        }

        private XDocument DownloadXmlForGuid(string guid)
        {
            string canvasUsername = ConfigurationManager.AppSettings["canvasUserName"];
            string canvasPassword = ConfigurationManager.AppSettings["canvasPassword"];
            string canvasUrl = "https://www.gocanvas.com/apiv2/submissions.xml?username=" + canvasUsername + "&password=" + canvasPassword + "&submission_guid=" + guid;
            try
            {
                XDocument retval = XDocument.Load(canvasUrl);
                return retval;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return null;
            }
        }

        private CanvasApiSubmission DeserialiseApiSubmission(XDocument xmlInput)
        {
            XElement firstNode = xmlInput.Element("submission-notification");

            CanvasApiSubmission retval = new CanvasApiSubmission();
            var serializer = new XmlSerializer(typeof(CanvasApiSubmission));

            try
            {
                retval = (CanvasApiSubmission)serializer.Deserialize(firstNode.Element("submission").CreateReader());
            }
            catch (Exception ex)
            {
                //Log error and XML
                System.Diagnostics.Trace.TraceError("Error occured serialising data: " + ex.ToString());
                System.Diagnostics.Trace.TraceError("XML: " + xmlInput.ToString());
                return null;
            }

            return retval;
        }

        private void ValidateEntities(List<CanvasRawData> canvasEntities)
        {
            //As this occurs server side automatically, then validation needs to 'fix' any issues with the entities and save them. 
            //These will be reviewed before final sheets are created.
            foreach (var itemToValidate in canvasEntities)
            {
                if (itemToValidate.Customer.Length >= 255)
                {
                    itemToValidate.Customer = itemToValidate.Customer.Substring(0, 254);
                }

                if (itemToValidate.AddressLine1.Length >= 255)
                {
                    itemToValidate.AddressLine1 = itemToValidate.AddressLine1.Substring(0, 254);
                }

                if (itemToValidate.AddressLine2.Length >= 255)
                {
                    itemToValidate.AddressLine2 = itemToValidate.AddressLine2.Substring(0, 254);
                }

                if(itemToValidate.TownCity.Length >= 255)
                {
                    itemToValidate.TownCity = itemToValidate.TownCity.Substring(0, 254);
                }

                if(itemToValidate.Postcode.Length >= 10)
                {
                    itemToValidate.Postcode = itemToValidate.Postcode.Substring(0, 9);
                }

                if(itemToValidate.CustomerContact.Length >= 100)
                {
                    itemToValidate.CustomerContact = itemToValidate.CustomerContact.Substring(0, 99);
                }

                if (itemToValidate.CustomerPhoneNo.Length >= 20)
                {
                    itemToValidate.CustomerPhoneNo = itemToValidate.CustomerPhoneNo.Substring(0, 19);
                }

                if (itemToValidate.MachineMakeModel.Length >= 100)
                {
                    itemToValidate.MachineMakeModel = itemToValidate.MachineMakeModel.Substring(0, 99);
                }

                if (itemToValidate.MachineSerial.Length >= 100)
                {
                    itemToValidate.MachineSerial = itemToValidate.MachineSerial.Substring(0, 99);
                }

                if (itemToValidate.CncControl.Length >= 100)
                {
                    itemToValidate.CncControl = itemToValidate.CncControl.Substring(0, 99);
                }

                if (itemToValidate.CustomerOrderNo.Length >= 100)
                {
                    itemToValidate.CustomerOrderNo = itemToValidate.CustomerOrderNo.Substring(0, 99);
                }

                if(itemToValidate.MttJobNumber.Length >= 20)
                {
                    itemToValidate.MttJobNumber = itemToValidate.MttJobNumber.Substring(0, 19);
                }

                if(itemToValidate.JobStatus.Length >= 100)
                {
                    itemToValidate.JobStatus = itemToValidate.JobStatus.Substring(0, 99);
                }

                if(itemToValidate.CustomerName.Length >= 255)
                {
                    itemToValidate.CustomerName = itemToValidate.CustomerName.Substring(0, 254);
                }

            }
            
        }

        private List<CanvasRawData> ConvertCanvasXmlEntityToDbEntity(CanvasRawXmlItem entitiyFromXml)
        {
            try
            {
                CanvasRawData retval = new CanvasRawData();
                retval.Approved = false;
                retval.CanvasResponseId = entitiyFromXml.CanvasResponseId;
                retval.DtDevice = Convert.ToDateTime(entitiyFromXml.DtDevice);
                retval.DtResponse = Convert.ToDateTime(entitiyFromXml.DtResponse);
                retval.SubmissionNumber = entitiyFromXml.SubmissionNumber;
                retval.UserFirstName = entitiyFromXml.UserFirstName;
                retval.UserSurname = entitiyFromXml.UserSurname;
                retval.Username = entitiyFromXml.UserName;
                //These are the dates for when canvas reports are run.  Not really relevent for xml files
                retval.DtStartSubmission = DateTime.Now;
                retval.DtEndSubmission = DateTime.Now;

                //Fill in the Job details section
                CompleteFormSection(retval, entitiyFromXml);
                CompleteJobDetailsSection(retval, entitiyFromXml);
                CompleteJobSignoffSection(retval, entitiyFromXml);

                //We need to create a copy of the return entity for each timesheet.  The rows are saved to the database as flat rows, therefore one row per day, to match the canvas data
                List<CanvasRawData> allEntities = CompleteTimeSheetSection(retval, entitiyFromXml);

                return allEntities;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return null;
            }
        }

        private void CompleteJobSignoffSection(CanvasRawData retval, CanvasRawXmlItem entitiyFromXml)
        {
            try
            {
                CanvasSections section = entitiyFromXml.SectionDetails.Where(sec => sec.Name.Equals("Job Signoff")).FirstOrDefault();
                CanvasScreen screen = section.CanvasScreenDetail.FirstOrDefault();
                //Job signoff section returns responses
                List<CanvasResponse> responses = screen.Responses;
                retval.JobTotalTimeOnsite = GetDoubleForLabel(responses, "JobTotalTimeOnsite");
                retval.JobTotalTravelTime = GetDoubleForLabel(responses, "JobTotalTravelTime");
                retval.JobTotalMileage = (int)GetDoubleForLabel(responses, "Total mileage");
                retval.TotalDailyAllowances = (int)GetDoubleForLabel(responses, "Total number of daily allowances");
                retval.TotalOvernightAllowances = (int)GetDoubleForLabel(responses, "Total number of overnight allowances");
                retval.TotalBarrierPayments = (int)GetDoubleForLabel(responses, "Total number of barrier payments");
                retval.JobStatus = GetStringValueForLabel(responses, "Job status");
                retval.FinalJobReport = GetStringValueForLabel(responses, "Final job report");
                retval.AdditionalFaults = GetStringValueForLabel(responses, "Additional faults found");
                retval.QuoteRequired = GetBooleanForLabel(responses, "Customer requires quote for follow-up work");
                retval.FollowUpPartsRequired = GetStringValueForLabel(responses, "Parts required for follow-up work");
                retval.Image1Url = GetStringValueForLabel(responses, "Image 1");
                retval.Image2Url = GetStringValueForLabel(responses, "Image 2");
                retval.Image3Url = GetStringValueForLabel(responses, "Image 3");
                retval.Image4Url = GetStringValueForLabel(responses, "Image 4");
                retval.Image5Url = GetStringValueForLabel(responses, "Image 5");
                retval.CustomerSignatureUrl = GetStringValueForLabel(responses, "Customer signature");
                retval.CustomerName = GetStringValueForLabel(responses, "Customer name");
                retval.DtSigned = GetDateValueForLabel(responses, "Date Signed");
                retval.MttEngSignatureUrl = GetStringValueForLabel(responses, "MTT engineer signature");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
        }

        private bool GetBooleanForLabel(List<CanvasResponse> responses, string label)
        {
            bool retval = false;
            try
            {
                string valueFound = responses.Where(m => m.ResponseLabel.Equals(label)).First().Value;
                if (valueFound.Equals("True"))
                {
                    retval = true;
                }
                else
                {
                    retval = false;
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                System.Diagnostics.Trace.TraceError("Label not found for: " + label);
            }
            return retval;
        }

        private int GetIntForLabel(List<CanvasResponse> responses, string label)
        {
            int retval = 0;
            try
            {
                string valueFound = responses.Where(m => m.ResponseLabel.Equals(label)).First().Value;
                retval = Convert.ToInt32(valueFound);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                System.Diagnostics.Trace.TraceError("Label not found for: " + label);
            }
            return retval;
        }

        private double GetDoubleForLabel(List<CanvasResponse> responses, string label)
        {
            double retval = 0;
            try
            {
                string valueFound = responses.Where(m => m.ResponseLabel.Equals(label)).First().Value;
                retval = Convert.ToDouble(valueFound);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                System.Diagnostics.Trace.TraceError("Label not found for: " + label);
            }
            return retval;
        }

        private List<CanvasRawData> CompleteTimeSheetSection(CanvasRawData templateEntity, CanvasRawXmlItem entitiyFromXml)
        {
            try
            {
                List<CanvasRawData> retval = new List<CanvasRawData>();
                CanvasSections section = entitiyFromXml.SectionDetails.Where(sec => sec.Name.Equals("Time Sheet")).FirstOrDefault();
                CanvasScreen screen = section.CanvasScreenDetail.FirstOrDefault();
                //There are multiple response groups for the timesheet section
                List<CanvasResponseGroup> responseGroups = screen.ResponseGroups;
                foreach (var responseGroup in responseGroups)
                {
                    CanvasRawData clonedItem = CanvasRawDataHelper.CopyEntity(templateEntity);
                    List<CanvasResponse> responses = responseGroup.SectionDetails.CanvasScreenDetail.First().Responses;
                    DateTime serviceDate = Convert.ToDateTime(responseGroup.Response.Value, new CultureInfo("en-GB"));
                    clonedItem.DtReport = serviceDate;
                    clonedItem.TravelStartTime = GetTimeValueForLabel(serviceDate, responses, "Travel time start");
                    clonedItem.ArrivalOnsiteTime = GetTimeValueForLabel(serviceDate, responses, "Arrival time on site");
                    clonedItem.DepartureSiteTime = GetTimeValueForLabel(serviceDate, responses, "Departure time from site");
                    clonedItem.TravelEndTime = GetTimeValueForLabel(serviceDate, responses, "Travel end time");
                    clonedItem.Mileage = GetIntForLabel(responses, "Mileage");
                    clonedItem.DailyAllowance = GetIntForLabel(responses, "Daily allowance");
                    clonedItem.OvernightAllowance = GetIntForLabel(responses, "Overnight allowance");
                    clonedItem.BarrierPayment = GetIntForLabel(responses, "Barrier payment");
                    clonedItem.TravelToSiteTime = GetDoubleForLabel(responses, "Travel time to site");
                    clonedItem.TravelFromSiteTime = GetDoubleForLabel(responses, "Travel time from site");
                    clonedItem.TotalTravelTime = GetDoubleForLabel(responses, "Total travel time");
                    clonedItem.TotalOnsiteTime = GetDoubleForLabel(responses, "Total time onsite");
                    clonedItem.DailyReport = GetStringValueForLabel(responses, "Daily report");
                    clonedItem.PartsSuppliedToday = GetStringValueForLabel(responses, "Parts supplied today");
                    retval.Add(clonedItem);
                }
                return retval;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return null;
            }
        }

        private DateTime GetTimeValueForLabel(DateTime serviceDate, List<CanvasResponse> responses, string label)
        {
            DateTime retval;
            try
            {
                string time = responses.Where(m => m.ResponseLabel.Equals(label)).First().Value;
                TimeSpan timeSet = Convert.ToDateTime(time).TimeOfDay;
                retval = serviceDate + timeSet;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                System.Diagnostics.Trace.TraceError("Label not found for: " + label);
                return new DateTime(1900, 1, 1);
            }
            return retval;
        }

        private void CompleteJobDetailsSection(CanvasRawData retval, CanvasRawXmlItem entitiyFromXml)
        {
            try
            {
                CanvasSections section = entitiyFromXml.SectionDetails.Where(sec => sec.Name.Equals("Job details")).FirstOrDefault();
                CanvasScreen screen = section.CanvasScreenDetail.FirstOrDefault();
                //The job details section returns responses
                List<CanvasResponse> responses = screen.Responses;
                retval.Customer = GetStringValueForLabel(responses, "Customer");
                retval.AddressLine1 = GetStringValueForLabel(responses, "Address line 1");
                retval.AddressLine2 = GetStringValueForLabel(responses, "Address line 2");
                retval.TownCity = GetStringValueForLabel(responses, "Town/City");
                retval.Postcode = GetStringValueForLabel(responses, "Postcode");
                retval.CustomerContact = GetStringValueForLabel(responses, "Customer contact");
                retval.CustomerPhoneNo = GetStringValueForLabel(responses, "Customer phone no.");
                retval.MachineMakeModel = GetStringValueForLabel(responses, "Machine make and model");
                retval.MachineSerial = GetStringValueForLabel(responses, "Machine serial no.");
                retval.CncControl = GetStringValueForLabel(responses, "CNC control");
                retval.DtJobStart = GetDateValueForLabel(responses, "Job start date");
                retval.CustomerOrderNo = GetStringValueForLabel(responses, "Customer order no.");
                retval.MttJobNumber = GetStringValueForLabel(responses, "MTT job no.");
                retval.JobDescription = GetStringValueForLabel(responses, "Job description");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
        }

        private DateTime GetDateValueForLabel(List<CanvasResponse> responses, string label)
        {
            DateTime retval;
            try
            {
                string valueFound = responses.Where(m => m.ResponseLabel.Equals(label)).First().Value;
                retval = Convert.ToDateTime(valueFound, new CultureInfo("en-GB"));
            }
            catch (FormatException fx)
            {
                System.Diagnostics.Trace.TraceError(fx.ToString());
                System.Diagnostics.Trace.TraceError("Date error for label: " + label);
                return new DateTime(1900, 1, 1);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                System.Diagnostics.Trace.TraceError("Label not found for: " + label);
                return new DateTime(1900, 1, 1);
            }
            return retval;
        }

        private string GetStringValueForLabel(List<CanvasResponse> responses, string label)
        {
            string retval = "";
            try
            {
                retval = responses.Where(m => m.ResponseLabel.Equals(label)).First().Value;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                System.Diagnostics.Trace.TraceError("Label not found for: " + label);
            }
            return retval;          
        }

        private void CompleteFormSection(CanvasRawData retval, CanvasRawXmlItem entitiyFromXml)
        {
            CanvasForm formDetails = entitiyFromXml.CanvasFormDetails.First();
            retval.AppName = formDetails.SubmissionFormName;
            retval.SubmissionFormVersion = formDetails.SubmissionFormVersion;
            retval.SubmissionFormName = formDetails.SubmissionFormName;
        }

        private List<CanvasRawXmlItem> DeserialiseCanvasData(XDocument xmlInput)
        {
            //Create XML Doc
            XElement canvasResultsElement = xmlInput.Element("CanvasResult");
            XElement submissionsElement = canvasResultsElement.Element("Submissions");

            List<CanvasRawXmlItem> retval = new List<CanvasRawXmlItem>();
            var serializer = new XmlSerializer(typeof(CanvasRawXmlItem));

            try
            {
                foreach (var submission in submissionsElement.Elements())
                {
                    CanvasRawXmlItem item = (CanvasRawXmlItem)serializer.Deserialize(submission.CreateReader());
                    retval.Add(item);
                }
            }
            catch (Exception ex)
            {
                //Log error and XML
                System.Diagnostics.Trace.TraceError("Error occured serialising data: " + ex.ToString());
                System.Diagnostics.Trace.TraceError("XML: " + xmlInput.ToString());
                return null;
            }

            return retval;

        }

        private XDocument GetXmlStreamFromFile()
        {
            XDocument retval = null;
            try
            {
                //retval = XDocument.Load("C:\\Users\\rtown\\Documents\\MTT\\IT\\Service Sheet.xml");
                retval = XDocument.Load("C:\\Users\\rtown\\Documents\\MTT\\IT\\Service Sheet - XML Test.xml");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error: " + ex.ToString());
            }

            return retval;
        }

        private XDocument GetXmlStream()
        {
            XDocument retval = null;
            try
            {
                if (HttpContext.IsDebuggingEnabled)
                {
                    var reader = new StreamReader(Request.InputStream);
                    var xmlString = reader.ReadToEnd();
                    retval = XDocument.Load(xmlString);
                }
                else
                {
                    var reader = new StreamReader(Request.InputStream);
                    //var xmlString = reader.ReadToEnd();
                    retval = XDocument.Load(reader);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Exception occured reading canvas XML: " + ex.ToString());
            }
            return retval;
        }
    }
}