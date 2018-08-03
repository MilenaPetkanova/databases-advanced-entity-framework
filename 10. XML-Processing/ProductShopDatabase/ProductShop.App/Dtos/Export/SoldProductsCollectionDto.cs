namespace ProductShop.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("sold-products")]
    public class SoldProductsCollectionDTO
    {
        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlElement("products")]
        public SoldProductAttributesDTO[] SoldProductDTOs { get; set; }
    }
}
