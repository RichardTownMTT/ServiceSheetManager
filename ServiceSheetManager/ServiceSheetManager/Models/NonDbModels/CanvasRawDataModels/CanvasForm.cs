using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels
{
    public class CanvasForm
    {
        [XmlElement(ElementName = "Name")]
        public string SubmissionFormName { get; set; }
        [XmlElement(ElementName = "Version")]
        public int SubmissionFormVersion { get; set; }
    }
}