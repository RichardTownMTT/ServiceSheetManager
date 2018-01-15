using ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ServiceSheetManager.Helpers
{
    public class CanvasSubmissionHelpers
    {
        public static XDocument DownloadXmlForGuid(string guid)
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
         
        public static CanvasApiSubmission DeserialiseApiSubmission(XDocument xmlInput)
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


    }
}