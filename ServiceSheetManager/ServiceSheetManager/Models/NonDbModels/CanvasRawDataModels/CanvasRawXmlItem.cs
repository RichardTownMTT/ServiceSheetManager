using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels
{
    [XmlRoot(ElementName = "Submission")]
    public class CanvasRawXmlItem
    {
        public CanvasRawXmlItem()
        {
            CanvasFormDetails = new List<CanvasForm>();
            SectionDetails = new List<CanvasSections>();
        }

        [XmlElement (ElementName = "SubmissionNumber")]
        public int SubmissionNumber { get; set; }
        [XmlElement(ElementName = "UserName")]
        public string UserName { get; set; }
        [XmlElement(ElementName = "FirstName")]
        public string UserFirstName { get; set; }
        [XmlElement(ElementName = "LastName")]
        public string UserSurname { get; set; }
        [XmlElement(ElementName = "ResponseID")]
        public string CanvasResponseId { get; set; }
        [XmlElement(ElementName = "Date")]
        public string DtResponse { get; set; }
        [XmlElement(ElementName = "DeviceDate")]
        public string DtDevice { get; set; }
        

        [XmlElement("Form")]
        public List<CanvasForm> CanvasFormDetails { get; set; }

        [XmlArray("Sections")]
        [XmlArrayItem(ElementName = "Section")]
        public List<CanvasSections> SectionDetails { get; set; }

        
        [XmlElement(ElementName = "Approved")]
        public bool Approved { get; set; }
    }
}