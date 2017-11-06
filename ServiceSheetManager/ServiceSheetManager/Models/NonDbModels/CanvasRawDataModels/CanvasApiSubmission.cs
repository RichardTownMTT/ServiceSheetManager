using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels
{
    [XmlRoot(ElementName = "submission")]
    public class CanvasApiSubmission
    {
        [XmlElement(ElementName = "guid")]
        public string guid { get; set; }
    }
}