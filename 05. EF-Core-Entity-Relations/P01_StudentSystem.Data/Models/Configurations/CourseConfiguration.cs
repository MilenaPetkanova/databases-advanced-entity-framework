namespace P01_StudentSystem.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_StudentSystem.Data.Models;

    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.CourseId);

            builder.Property(c => c.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(80);

            builder.Property(c => c.Description)
                .IsRequired(false)
                .IsUnicode();

            builder.Property(c => c.StartDate)
                .IsRequired()
                .HasColumnType("DATETIME2");

            builder.Property(c => c.EndDate)
                .IsRequired()
                .HasColumnType("DATETIME2");

            builder.Property(c => c.Price)
                .IsRequired();
        }
    }
}