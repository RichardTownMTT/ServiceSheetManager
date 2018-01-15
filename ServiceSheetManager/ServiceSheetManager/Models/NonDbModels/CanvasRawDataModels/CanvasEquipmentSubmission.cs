using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels
{
    [XmlRoot(ElementName = "Submission")]
    public class CanvasEquipmentSubmission
    {
        public CanvasEquipmentSubmission()
        {
            Sections = new List<CanvasSection>();
        }

        [XmlElement(ElementName = "SubmissionNumber")]
        public int SubmissionNumber { get; set; }
        [XmlElement(ElementName = "Date")]
        public string DateScanned { get; set; }

        [XmlArray("Sections")]
        [XmlArrayItem(ElementName = "Section")]
        public List<CanvasSection> Sections { get; set; }

        [XmlElement(ElementName = "UserName")]
        public string Username { get; set; }

        [XmlElement(ElementName = "FirstName")]
        public string UserFirstName { get; set; }

        [XmlElement(ElementName = "LastName")]
        public string UserSurname { get; set; }

    }
}