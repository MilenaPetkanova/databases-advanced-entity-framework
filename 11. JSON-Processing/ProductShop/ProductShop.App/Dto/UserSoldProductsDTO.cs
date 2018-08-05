namespace ProductShop.App.Dto
{
    using Newtonsoft.Json;

    public class UserSoldProductsDTO
    {
        [JsonProperty("fistName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("productsSold")]
        public SoldProductAndBuyerInfoDTO[] ProductsSold { get; set; }
    }
}
