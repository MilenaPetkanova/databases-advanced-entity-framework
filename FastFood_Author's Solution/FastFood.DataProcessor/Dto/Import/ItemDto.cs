using System.ComponentModel.DataAnnotations;

namespace FastFood.DataProcessor.Dto.Import
{
    public class ItemDto
    {
	    [StringLength(30, MinimumLength = 3)]
	    public string Name { get; set; }

	    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
	    public decimal Price { get; set; }

		[Required]
	    [StringLength(30, MinimumLength = 3)]
	    public string Category { get; set; }
	}
}
