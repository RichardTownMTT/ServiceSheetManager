using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels
{
    [XmlRoot(ElementName = "Section")]
    public class CanvasScreen
    {
        public CanvasScreen()
        {
            Responses = new List<CanvasResponse>();
        }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlArray(ElementName = "Responses")]
        [XmlArrayItem(ElementName = "Response")]
        public List<CanvasResponse> Responses { get; set; }

        //Timesheets have a ResponseGroup
        [XmlArray(ElementName = "ResponseGroups")]
        [XmlArrayItem(ElementName = "ResponseGroup")]
        public List<CanvasResponseGroup> ResponseGroups { get; set; }
    }
}