namespace ProductShop.App.Dtos.Import
{
    using System.Xml.Serialization;
    using System.ComponentModel.DataAnnotations;

    [XmlType("user")]
    public class UserImportDto
    {
        [XmlAttribute("firstName")]
        public string FirstName { get; set; }

        [XmlAttribute("lastName")]
        [MinLength(3)]
        public string LastName { get; set; }

        [XmlAttribute("age")]
        public int Age { get; set; }
    }
}
