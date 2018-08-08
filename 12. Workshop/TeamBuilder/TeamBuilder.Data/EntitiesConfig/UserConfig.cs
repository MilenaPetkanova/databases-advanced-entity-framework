namespace TeamBuilder.Data.EntitiesConfig
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using TeamBuilder.Models;

    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasAlternateKey(u => u.Username);

            builder.HasMany(u => u.CreatedTeams)
                .WithOne(t => t.Creator)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.CreatedEvents)
                .WithOne(e => e.Creator)
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.ReceivedInvitations)
                .WithOne(i => i.InvitedUser)
                .HasForeignKey(i => i.InvitedUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
