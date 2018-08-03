namespace CarDealer.App.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlType("part")]
    public class PartAttributesDTO
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }
}
