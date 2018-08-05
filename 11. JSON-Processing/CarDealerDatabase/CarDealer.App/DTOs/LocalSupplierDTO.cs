namespace CarDealer.App.DTOs
{
    using Newtonsoft.Json;

    public class LocalSupplierDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("PartsCount")]
        public int PartsCount { get; set; }
    }
}
