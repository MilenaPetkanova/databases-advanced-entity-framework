namespace TeamBuilder.Data.EntitiesConfig
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using TeamBuilder.Models;

    public class TeamConfig : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasAlternateKey(t => t.Name);
        }
    }
}
