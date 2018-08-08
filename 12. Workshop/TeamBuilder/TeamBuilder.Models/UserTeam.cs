namespace TeamBuilder.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserTeam
    {
        public int UserId { get; set; }

        [Required]
        public User User { get; set; }

        public int TeamId { get; set; }

        [Required]
        public Team Team { get; set; }
    }
}
