namespace ProductShop.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("category")]
    public class CategoryProductsInfoDTO
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("products-count")]
        public int ProductsCount { get; set; }

        [XmlElement("average-price")]
        public decimal AveragePrice { get; set; }

        [XmlElement("total-revenue")]
        public decimal TotalRevenue { get; set; }
    }
}
