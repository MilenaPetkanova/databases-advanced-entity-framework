namespace FastFood.DataProcessor.Dto.Import
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Order")]
    public class OrderDTO
    {
        [Required]
        public string Customer { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public string DateTime { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [XmlArray("Items")]
        public OrderItemDTO[] Items { get; set; }
    }
}

