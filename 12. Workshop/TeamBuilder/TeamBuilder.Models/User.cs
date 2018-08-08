namespace TeamBuilder.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TeamBuilder.Models.Enums;

    public class User
    {
        public User()
        {
            this.CreatedEvents = new List<Event>();
            this.CreatedTeams = new List<Team>();
            this.UserTeams = new List<UserTeam>();
            this.ReceivedInvitations = new List<Invitation>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Username { get; set; }

        [MaxLength(25)]
        public string FirstName { get; set; }

        [MaxLength(25)]
        public string LastName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6)]
        [RegularExpression(@"([A-Z]+(.+)[\d]+)|([\d]+(.+)[A-Z]+)", ErrorMessage = "Invalid password.")]
        public string Password { get; set; }

        public Gender Gender { get; set; }

        [Range(0, int.MaxValue)]
        public int Age { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Event> CreatedEvents { get; set; }

        public ICollection<Team> CreatedTeams { get; set; }

        public ICollection<UserTeam> UserTeams { get; set; }

        public ICollection<Invitation> ReceivedInvitations { get; set; }
    }
}
