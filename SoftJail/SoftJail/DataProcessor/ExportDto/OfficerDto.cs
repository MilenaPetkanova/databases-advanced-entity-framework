using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.DataProcessor.ExportDto
{
    public class OfficerDto
    {
        [JsonProperty("OfficerName")]
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string FullName { get; set; }

        public string Department { get; set; }
    }
}