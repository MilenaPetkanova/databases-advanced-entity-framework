namespace ProductShop.App.Dto
{
    using Newtonsoft.Json;

    public class SoldProductDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}