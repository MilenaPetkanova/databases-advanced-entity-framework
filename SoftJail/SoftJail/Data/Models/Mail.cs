using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
    public class Mail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\d\s].*str.$")]
        public string Address { get; set; }

        public int PrisonerId { get; set; }
        [Required]
        public Prisoner Prisoner { get; set; }
    }
}
