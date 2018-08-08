namespace TeamBuilder.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Team
    {
        public Team()
        {
            this.UserTeams = new List<UserTeam>();
            this.TeamEvents = new List<TeamEvent>();
            this.InvitationsSent = new List<Invitation>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }

        [MaxLength(32)]
        public string Description { get; set; }

        [Required]
        [StringLength(3)]
        public string Acronym { get; set; }

        public int CreatorId { get; set; }

        [Required]
        public User Creator { get; set; }

        public ICollection<UserTeam> UserTeams { get; set; }

        public ICollection<TeamEvent> TeamEvents { get; set; }

        public ICollection<Invitation> InvitationsSent { get; set; }
    }
}
