namespace CarDealer.App.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlType("car")]
    public class CarPartsDTO
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }

        [XmlArray("parts")]
        public PartAttributesDTO[] CarParts { get; set; }
    }
}
