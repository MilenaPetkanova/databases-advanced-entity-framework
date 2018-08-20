using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SoftJail.DataProcessor.ExportDto
{
    public class PrisonerDto
    {
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string FullName { get; set; }

        public int CellNumber { get; set; }

        public OfficerDto[] Officers { get; set; }

        public decimal TotalOfficerSalary { get; set; }
    }
}
