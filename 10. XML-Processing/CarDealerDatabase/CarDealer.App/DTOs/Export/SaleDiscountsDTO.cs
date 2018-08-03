namespace CarDealer.App.DTOs.Export
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("sale")]
    public class SaleDiscountsDTO
    {
        [XmlElement("car")]
        public CarFromSaleDTO Car { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [Required]
        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set; }
    }
}
