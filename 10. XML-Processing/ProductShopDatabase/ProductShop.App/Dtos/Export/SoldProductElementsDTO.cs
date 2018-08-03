namespace ProductShop.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("product")]
    public class SoldProductElementsDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
