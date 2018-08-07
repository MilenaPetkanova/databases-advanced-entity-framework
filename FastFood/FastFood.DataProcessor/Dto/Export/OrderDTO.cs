namespace FastFood.DataProcessor.Dto.Export
{
    using Newtonsoft.Json;

    public class OrderDTO
    {
        public string Customer { get; set; }

        [JsonProperty("Items")]
        public ItemDTO[] Items { get; set; }

        public decimal TotalPrice { get; set; }
    }
}