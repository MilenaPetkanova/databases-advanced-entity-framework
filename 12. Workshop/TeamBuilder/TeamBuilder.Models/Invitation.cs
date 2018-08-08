namespace TeamBuilder.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Invitation
    {
        public Invitation()
        {
            this.IsActive = true;
        }

        [Key]
        public int Id { get; set; }

        [Range(0, int.MaxValue)]
        public int InvitedUserId { get; set; }

        [Required]
        public User InvitedUser { get; set; }

        [Range(0, int.MaxValue)]
        public int TeamId { get; set; }

        [Required]
        public Team Team { get; set; }

        public bool IsActive { get; set; }
    }
}
