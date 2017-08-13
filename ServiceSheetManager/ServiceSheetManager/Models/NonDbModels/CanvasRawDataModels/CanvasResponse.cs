using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels
{
    [XmlRoot(ElementName = "Response")]
    public class CanvasResponse
    {
        [XmlElement(ElementName = "Label")]
        public string ResponseLabel { get; set; }
        [XmlElement(ElementName = "Value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "Type")]
        public string Type { get; set; }
    }
}