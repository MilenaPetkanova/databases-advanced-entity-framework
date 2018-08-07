namespace FastFood.DataProcessor.Dto.Import
{
    using System.Xml.Serialization;
    using System.ComponentModel.DataAnnotations;

    [XmlType("Item")]
    public class OrderItemDTO
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        public int Quantity { get; set; }
    }
}
