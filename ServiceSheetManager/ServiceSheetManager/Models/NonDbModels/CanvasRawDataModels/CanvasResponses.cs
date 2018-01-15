using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels
{
    public class CanvasResponses
    {
        public CanvasResponses()
        {
            Responses = new List<CanvasResponse>();
        }

        [XmlElement(ElementName = "Response")]
        public CanvasResponse Response { get; set; }

        [XmlArray(ElementName = "Responses")]
        [XmlArrayItem(ElementName = "Response")]
        public List<CanvasResponse> Responses { get; set; }
    }
}