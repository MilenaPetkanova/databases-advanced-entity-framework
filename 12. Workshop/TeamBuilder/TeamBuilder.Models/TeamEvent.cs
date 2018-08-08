namespace TeamBuilder.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TeamEvent
    {
        public int TeamId { get; set; }

        [Required]
        public Team Team { get; set; }

        public int EventId { get; set; }

        [Required]
        public Event Event { get; set; }
    }
}
