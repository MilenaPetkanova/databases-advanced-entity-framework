namespace CarDealer.App.DTOs
{
    using System;
    using Newtonsoft.Json;

    public class CustomerDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("BirthDate")]
        public DateTime BirthDate { get; set; }

        [JsonProperty("IsYoungDriver")]
        public bool IsYoungDriver { get; set; }

        [JsonProperty("SalesCount")]
        public int SalesCount { get; set; }
    }
}
