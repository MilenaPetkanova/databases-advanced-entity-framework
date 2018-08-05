namespace ProductShop.App.Dto
{
    using Newtonsoft.Json;

    public class UserCollectionDTO
    {
        [JsonProperty("usersCount")]
        public int UsersCount { get; set; }

        [JsonProperty("users")]
        public UserDTO[] Users { get; set; }
    }
}
