namespace FastFood.DataProcessor.Dto.Export
{
    using Newtonsoft.Json;

    public class EmployeeOrdersDTO
    {
        public string Name { get; set; }

        [JsonProperty("Orders")]
        public OrderDTO[] Orders { get; set; }

        public decimal TotalMade { get; set; }
    }
}
