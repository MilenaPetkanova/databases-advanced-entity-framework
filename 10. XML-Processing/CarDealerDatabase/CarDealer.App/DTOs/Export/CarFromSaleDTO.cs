namespace CarDealer.App.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlType("car")]
    //[XmlElement("car")]
    public class CarFromSaleDTO
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}
