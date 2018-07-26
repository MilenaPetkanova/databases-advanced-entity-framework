namespace P01_StudentSystem.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_StudentSystem.Data.Models;

    public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
    {
        public void Configure(EntityTypeBuilder<StudentCourse> builder)
        {
            builder.HasKey(sc => new { sc.StudentId, sc.CourseId });

            builder.HasOne(s => s.Course)
                .WithMany(c => c.StudentsEnrolled)
                .HasForeignKey(s => s.CourseId);

            builder.HasOne(c => c.Student)
                .WithMany(s => s.CourseEnrollments)
                .HasForeignKey(c => c.StudentId);
        }
    }
}