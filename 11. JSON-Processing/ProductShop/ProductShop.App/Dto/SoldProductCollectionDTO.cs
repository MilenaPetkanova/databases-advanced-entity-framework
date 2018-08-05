namespace ProductShop.App.Dto
{
    using Newtonsoft.Json;

    public class SoldProductCollectionDTO
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("products")]
        public SoldProductDTO[] Products { get; set; }
    }
}