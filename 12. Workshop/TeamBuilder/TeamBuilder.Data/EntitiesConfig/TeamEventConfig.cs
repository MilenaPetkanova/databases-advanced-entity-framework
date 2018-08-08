namespace TeamBuilder.Data.EntitiesConfig
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using TeamBuilder.Models;

    public class TeamEventConfig : IEntityTypeConfiguration<TeamEvent>
    {
        public void Configure(EntityTypeBuilder<TeamEvent> builder)
        {
            builder.HasKey(te => new { te.TeamId, te.EventId });

            builder.HasOne(ut => ut.Team)
                .WithMany(t => t.TeamEvents)
                .HasForeignKey(ut => ut.TeamId);

            builder.HasOne(ut => ut.Event)
                .WithMany(e =>e.TeamEvents)
                .HasForeignKey(ut => ut.EventId);
        }
    }
}
