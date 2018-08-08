namespace TeamBuilder.Data
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using TeamBuilder.Models;
    using TeamBuilder.Data.EntitiesConfig;

    public class TeamBuilderContext : DbContext
    {
        public TeamBuilderContext()
        {
        }

        public TeamBuilderContext(DbContextOptions options)
            :base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }
        public DbSet<TeamEvent> TeamEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.StringConnection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new EventConfig());
            modelBuilder.ApplyConfiguration(new TeamConfig());
            modelBuilder.ApplyConfiguration(new UserTeamConfig());
            modelBuilder.ApplyConfiguration(new TeamEventConfig());
        }
    }
}
