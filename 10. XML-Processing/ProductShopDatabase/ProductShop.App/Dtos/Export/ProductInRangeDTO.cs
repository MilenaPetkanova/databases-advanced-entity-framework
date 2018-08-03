namespace ProductShop.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("product")]
    public class ProductInRangeDTO
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }

        [XmlAttribute("buyer")]
        public string BuyerFullName { get; set; }
    }
}
