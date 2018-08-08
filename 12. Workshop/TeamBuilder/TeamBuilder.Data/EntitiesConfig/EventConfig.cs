namespace TeamBuilder.Data.EntitiesConfig
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using TeamBuilder.Models;

    public class EventConfig : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.Property(e => e.Name)
                .IsUnicode();

            builder.Property(e => e.Description)
                .IsUnicode();
        }
    }
}
