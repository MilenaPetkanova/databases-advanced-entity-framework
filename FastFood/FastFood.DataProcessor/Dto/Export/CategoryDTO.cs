namespace FastFood.DataProcessor.Dto.Export
{
    using System.Xml.Serialization;

    [XmlType("Category")]
    public class CategoryDTO
    {
        public string Name { get; set; }

        [XmlElement("MostPopularItem")]
        public CategoryItemDTO MostPopularItem { get; set; }
    }
}
