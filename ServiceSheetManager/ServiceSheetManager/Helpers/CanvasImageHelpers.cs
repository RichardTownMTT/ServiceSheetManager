using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace ServiceSheetManager.Helpers
{
    public static class CanvasImageHelpers
    {

        public static string LoadCanvasImageForUrlPdfVersion(string imageUrl)
        {
            string base64Image = LoadCanvasImageForUrl(imageUrl);

            if (!String.IsNullOrEmpty(base64Image))
            {
                return "base64:" + base64Image;
            }
            else
            {
                return null;
            }
        }
        public static string LoadCanvasImageForUrl(string imageUrl)
        {
            if (String.IsNullOrEmpty(imageUrl))
            {
                return null;
            }

            string canvasUrl = "";
            //Credentials from canvas.  
            string canvasUsername = ConfigurationManager.AppSettings["canvasUserName"];
            string canvasPassword = ConfigurationManager.AppSettings["canvasPassword"];

            //Canvas images are 10 digit numbers if we've stored the number only.  Add a few for expansion
            if (imageUrl.Length < 15)
            {
                canvasUrl = "https://www.gocanvas.com/apiv2/images.xml?image_id=" + imageUrl + "&username=" + canvasUsername + "&password=" + canvasPassword;
            }
            else
            {
                int lastSlash = imageUrl.LastIndexOf("/");
                string imageId = imageUrl.Substring(lastSlash + 1);
                canvasUrl = "https://www.gocanvas.com/apiv2/images.xml?image_id=" + imageId + "&username=" + canvasUsername + "&password=" + canvasPassword;
            }

            try
            {
                WebClient wc = new WebClient();
                byte[] downloadedData = wc.DownloadData(canvasUrl);

                string errorCode = CanvasErrorCode(downloadedData);
                if (!errorCode.Equals(""))
                {
                    return null;
                }

                MemoryStream ms = new MemoryStream(downloadedData);
                ms.Close();

                byte[] byteArray = ms.ToArray();

                //return "base64:" + Convert.ToBase64String(byteArray);
                return Convert.ToBase64String(byteArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private static string CanvasErrorCode(byte[] downloadedData)
        {
            //This method returns the canvas error code, or null if the data is correct.
            try
            {
                string response = Encoding.ASCII.GetString(downloadedData);
                XDocument xDoc = XDocument.Parse(response);
                return ValidateCanvasXml(xDoc.Root);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //Unable to parse canvas error, so return null.
                return "";
            }
        }

        private static string ValidateCanvasXml(XElement rootElement)
        {
            string retval = "";
            string errorMessage = "";
            XElement resultXML = rootElement.Element("Error");

            if (resultXML != null)
            {
                retval = resultXML.Element("ErrorCode").Value;
                errorMessage = resultXML.Element("Description").Value;
            }

            if (!retval.Equals(""))
            {
                Console.WriteLine(errorMessage);
            }

            return retval;
        }
    }
}