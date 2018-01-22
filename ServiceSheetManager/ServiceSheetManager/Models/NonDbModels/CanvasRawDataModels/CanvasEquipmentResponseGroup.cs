using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels
{
    [XmlRoot(ElementName = "ResponseGroup")]
    public class CanvasEquipmentResponseGroup
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Response")]
        public CanvasResponse Response { get; set; }

        [XmlElement(ElementName = "Section")]
        public CanvasSections SectionDetails { get; set; }
    }
}