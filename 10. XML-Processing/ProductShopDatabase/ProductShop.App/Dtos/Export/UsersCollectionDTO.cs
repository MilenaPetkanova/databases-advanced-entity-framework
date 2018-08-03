namespace ProductShop.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlRoot("users")]
    public class UsersCollectionDTO
    {
        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlElement("user")]
        public UserDTO[] UserDTOs { get; set; }
    }
}
