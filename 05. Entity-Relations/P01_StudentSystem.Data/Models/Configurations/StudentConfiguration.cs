namespace P01_StudentSystem.Data.Models.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.StudentId);

            builder.Property(s => s.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(100);

            builder.Property(s => s.PhoneNumber)
                .IsRequired(false)
                .IsUnicode(false)
                .HasColumnType("CHAR(10)");

            builder.Property(s => s.RegisteredOn)
                .IsRequired()
                .HasColumnType("DATETIME2");

            builder.Property(s => s.Birthday)
                .IsRequired(false)
                .HasColumnType("DATETME2");
        }
    }
}
