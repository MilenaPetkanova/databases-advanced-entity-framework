namespace P01_StudentSystem.Data
{
    using Microsoft.EntityFrameworkCore;
    using P01_StudentSystem.Data.Configurations;
    using P01_StudentSystem.Data.Models;

    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {
        }

        public StudentSystemContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<StudentCourse> StudentsCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.CourseId);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(80);

                entity.Property(c => c.Description)
                    .IsRequired(false)
                    .IsUnicode();

                entity.Property(c => c.StartDate)
                    .IsRequired()
                    .HasColumnType("DATETIME2");

                entity.Property(c => c.EndDate)
                    .IsRequired()
                    .HasColumnType("DATETIME2");

                entity.Property(c => c.Price)
                    .IsRequired();
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentId);

                entity.Property(s => s.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(100);

                entity.Property(s => s.PhoneNumber)
                    .IsRequired(false)
                    .IsUnicode(false)
                    .HasColumnType("CHAR(10)");

                entity.Property(s => s.RegisteredOn)
                    .IsRequired()
                    .HasColumnType("DATETIME2");

                entity.Property(s => s.Birthday)
                    .IsRequired(false)
                    .HasColumnType("DATE");
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(r => r.ResourceId);

                entity.Property(r => r.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(50);

                entity.Property(r => r.Url)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(r => r.ResourceType)
                    .IsRequired();

                entity.HasOne(r => r.Course)
                    .WithMany(c => c.Resources)
                    .HasForeignKey(r => r.CourseId);
            });

            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasKey(h => h.HomeworkId);

                entity.Property(h => h.Content)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(h => h.ContentType)
                    .IsRequired();

                entity.Property(h => h.SubmissionTime)
                    .IsRequired()
                    .HasColumnType("DATETIME2");

                entity.HasOne(h => h.Student)
                    .WithMany(s => s.HomeworkSubmissions)
                    .HasForeignKey(h => h.StudentId);

                entity.HasOne(h => h.Course)
                    .WithMany(c => c.HomeworkSubmissions)
                    .HasForeignKey(h => h.CourseId);
            });

            modelBuilder.Entity<StudentCourse>(entity => 
            {
                entity.HasKey(sc => new { sc.StudentId, sc.CourseId });

                entity.HasOne(s => s.Course)
                    .WithMany(c => c.StudentsEnrolled)
                    .HasForeignKey(s => s.CourseId);

                entity.HasOne(c => c.Student)
                    .WithMany(s => s.CourseEnrollments)
                    .HasForeignKey(c => c.StudentId);
            });

                //modelBuilder.ApplyConfiguration(new StudentCourseConfiguration());
                //modelBuilder.ApplyConfiguration(new CourseConfiguration());
                //modelBuilder.ApplyConfiguration(new ResourceConfiguration());
                //modelBuilder.ApplyConfiguration(new HomeworkConfiguration());
                //modelBuilder.ApplyConfiguration(new StudentCourseConfiguration());
            }
    }
}
