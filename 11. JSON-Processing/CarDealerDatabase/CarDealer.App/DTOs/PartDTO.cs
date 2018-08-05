namespace CarDealer.App.DTOs
{
    using Newtonsoft.Json;

    [JsonObject("user")]
    public class PartDTO
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Price")]
        public decimal Price { get; set; }
    }
}
