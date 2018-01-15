using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels
{
    [XmlRoot("Section")]
    public class CanvasSection
    {
        public CanvasSection()
        {
            CanvasScreenDetail = new List<CanvasScreen>();
        }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlArray("Screens")]
        [XmlArrayItem(ElementName = "Screen")]
        public List<CanvasScreen> CanvasScreenDetail { get; set; }
    }
}