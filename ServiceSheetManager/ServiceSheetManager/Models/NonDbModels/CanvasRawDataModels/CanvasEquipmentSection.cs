using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels
{
    [XmlRoot("Section")]
    public class CanvasEquipmentSection
    {
        public CanvasEquipmentSection()
        {
            CanvasScreenDetail = new List<CanvasEquipmentScreen>();
        }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlArray("Screens")]
        [XmlArrayItem(ElementName = "Screen")]
        public List<CanvasEquipmentScreen> CanvasScreenDetail { get; set; }
    }
}