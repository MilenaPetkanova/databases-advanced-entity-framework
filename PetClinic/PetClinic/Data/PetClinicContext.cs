namespace PetClinic.Data
{
    using Microsoft.EntityFrameworkCore;
    using PetClinic.Models;

    public class PetClinicContext : DbContext
    {
        public PetClinicContext() { }

        public PetClinicContext(DbContextOptions options)
            :base(options) { }

        public DbSet<Passport> Passports { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<ProcedureAnimalAid> ProceduresAnimalAids { get; set; }
        public DbSet<AnimalAid> AnimalAids { get; set; }
        public DbSet<Vet> Vets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProcedureAnimalAid>()
                .HasKey(paa => new { paa.ProcedureId, paa.AnimalAidId });

            builder.Entity<ProcedureAnimalAid>()
                .HasOne(ppa => ppa.Procedure)
                .WithMany(p => p.ProcedureAnimalAids)
                .HasForeignKey(paa => paa.ProcedureId);

            builder.Entity<ProcedureAnimalAid>()
                .HasOne(ppa => ppa.AnimalAid)
                .WithMany(aa => aa.AnimalAidProcedures)
                .HasForeignKey(paa => paa.AnimalAidId);

            builder.Entity<Procedure>()
                .HasOne(p => p.Vet)
                .WithMany(v => v.Procedures)
                .HasForeignKey(p => p.VetId);

            builder.Entity<Procedure>()
                .HasOne(p => p.Animal)
                .WithMany(a => a.Procedures)
                .HasForeignKey(p => p.AnimalId);

            builder.Entity<Vet>()
                .HasAlternateKey(v => v.PhoneNumber);

            builder.Entity<AnimalAid>()
                .HasAlternateKey(aa => aa.Name);
        }
    }
}
