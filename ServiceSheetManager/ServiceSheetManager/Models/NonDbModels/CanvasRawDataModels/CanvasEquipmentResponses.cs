using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels
{
    public class CanvasEquipmentResponses
    {
        public CanvasEquipmentResponses()
        {
            Responses = new List<CanvasEquipmentResponse>();
        }

        [XmlArray(ElementName = "Responses")]
        [XmlArrayItem(ElementName = "Response")]
        public List<CanvasEquipmentResponse> Responses { get; set; }

        [XmlElement(ElementName = "Response")]
        public CanvasEquipmentResponse SingleResponse { get; set; }
    }
}