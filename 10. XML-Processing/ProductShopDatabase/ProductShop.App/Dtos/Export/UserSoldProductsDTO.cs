namespace ProductShop.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("user")]
    public class UserSoldProductsDTO
    {
        [XmlAttribute("first-name")]
        public string FirstName { get; set; }

        [XmlAttribute("last-name")]
        public string LastName { get; set; }

        [XmlArray("sold-products")]
        public SoldProductElementsDTO[] SoldProducts { get; set; }
    }
}
