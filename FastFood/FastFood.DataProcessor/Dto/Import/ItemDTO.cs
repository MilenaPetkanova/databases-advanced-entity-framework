namespace FastFood.DataProcessor.Dto.Import
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class ItemDTO
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Category { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }
    }
}
