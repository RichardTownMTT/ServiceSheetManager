using ServiceSheetManager.Helpers;
using ServiceSheetManager.Models;
using ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ServiceSheetManager.Controllers
{
    public class CanvasEquipmentSubmissionController : Controller
    {
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        [HttpPost]
        public async Task<HttpResponseMessage> Submit()
        {
            HttpResponseMessage response;

            XDocument xmlInput = null;
            if (HttpContext.IsDebuggingEnabled)
            {
                xmlInput = GetXmlStreamFromFile();
            }
            else
            {
                xmlInput = GetXmlFromStream();
            }

            if (xmlInput == null)
            {
                response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                return response;
            }

            CanvasApiSubmission apiSubmission = CanvasSubmissionHelpers.DeserialiseApiSubmission(xmlInput);
            System.Diagnostics.Trace.TraceError("Guid = " + apiSubmission.guid);

            XDocument canvasDataXml = CanvasSubmissionHelpers.DownloadXmlForGuid(apiSubmission.guid);

            if (canvasDataXml == null)
            {
                response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                System.Diagnostics.Trace.TraceError("Canvas Data XML null");
                return response;
            }

            List<CanvasEquipmentSubmission> submissions = DeserialiseCanvasData(canvasDataXml);

            if (submissions == null)
            {
                response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                System.Diagnostics.Trace.TraceError("Submissions null");
                return response;
            }

            //Process the equipment locations.  Check equipment exists and save location
            List<EquipmentLocation> locationEntities = await CanvasEquipmentHelper.ProcessEquipmentLocationSubmissions(submissions, db);

            if (locationEntities == null)
            {
                response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                System.Diagnostics.Trace.TraceError("Location Entities null");
                return response;
            }

            try
            {
                db.EquipmentLocations.AddRange(locationEntities);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error occured while saving from canvas: " + ex.ToString());
                response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                return response;
            }


            response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            return response;
        }

        private XDocument GetXmlStreamFromFile()
        {
            XDocument retval = null;
            try
            {
                retval = XDocument.Load("C:\\Users\\rtown\\Desktop\\Equipment\\EquipmentSubmission.xml");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error: " + ex.ToString());
            }

            return retval;
        }

        private XDocument GetXmlFromStream()
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

        private List<CanvasEquipmentSubmission> DeserialiseCanvasData(XDocument xmlInput)
        {
            XElement canvasResultsElement = xmlInput.Element("CanvasResult");
            XElement submissionsElement = canvasResultsElement.Element("Submissions");

            List<CanvasEquipmentSubmission> retval = new List<CanvasEquipmentSubmission>();
            var serializer = new XmlSerializer(typeof(CanvasEquipmentSubmission));

            try
            {
                foreach (var submission in submissionsElement.Elements())
                {
                    CanvasEquipmentSubmission item = (CanvasEquipmentSubmission)serializer.Deserialize(submission.CreateReader());
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
    }
}