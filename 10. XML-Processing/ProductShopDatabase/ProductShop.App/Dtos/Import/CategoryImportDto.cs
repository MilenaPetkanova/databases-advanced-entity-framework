namespace ProductShop.App.Dtos.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("category")]
    public class CategoryImportDto
    {
        [XmlElement("name")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Name must have min Length of 3 and max Length of 15.")]
        public string Name { get; set; }
    }
}
