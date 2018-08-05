namespace CarDealer.App.DTOs
{
    using Newtonsoft.Json;

    [JsonObject("car")]
    public class CarPartsDTO
    {
        [JsonProperty("Make")]
        public string Make { get; set; }

        [JsonProperty("Model")]
        public string Model { get; set; }

        [JsonProperty("TravelledDistance")]
        public long TravelledDistance { get; set; }

        [JsonProperty("parts")]
        public PartDTO[] CarParts { get; set; }
    }
}
