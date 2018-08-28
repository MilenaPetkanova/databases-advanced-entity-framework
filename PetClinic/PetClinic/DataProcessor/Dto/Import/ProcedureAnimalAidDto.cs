using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Import
{
    [XmlType("AnimalAid")]
    public class ProcedureAnimalAidDto
    {
        public string Name { get; set; }
    }
}