using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ExportDto
{
    [XmlType("Message")]
    public class MessageDto
    {
        public string Description { get; set; }
    }
}