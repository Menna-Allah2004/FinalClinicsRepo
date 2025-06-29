using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedicalConnect.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("YourConnectionStringHere");
            }
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalReport> MedicalReports { get; set; }
        public DbSet<ContactUsMessage> ContactUsMessages { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ========== User Relations (Identity) ==========

            // Doctor-User (One-to-One)
            builder.Entity<Doctor>()
            .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient-User (One-to-One)  
            builder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== Doctor Relations ==========

            // Patient-Doctor (Many-to-One)
            builder.Entity<Patient>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(p => p.PrimaryDoctorId)
                .OnDelete(DeleteBehavior.Restrict); // إذا انحذف الدكتور، المرضى يبقوا بس DoctorId = null

            // Appointment-Doctor (Many-to-One)
            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade); // إذا انحذف الدكتور، مواعيده تنحذف

            // Rating-Doctor (Many-to-One)
            builder.Entity<Rating>()
                .HasOne(r => r.Doctor)
                .WithMany(d => d.Ratings)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Cascade); // إذا انحذف الدكتور، تقييماته تنحذف

            // MedicalReport-Doctor (Many-to-One)
            builder.Entity<MedicalReport>()
                .HasOne(m => m.Doctor)
                .WithMany(d => d.MedicalReports)
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.Restrict); // التقارير الطبية مهمة ما تنحذف

            // ========== Patient Relations ==========

            // Appointment-Patient (Many-to-One)
            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            // Rating-Patient (Many-to-One)
            builder.Entity<Rating>()
                .HasOne(r => r.Patient)
                .WithMany(p => p.Ratings)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // MedicalReport-Patient (Many-to-One)
            builder.Entity<MedicalReport>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalReports)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            // ========== Other Relations ==========

            // MedicalReport-Appointment (One-to-One)
            builder.Entity<MedicalReport>()
                .HasOne(m => m.Appointment)
                .WithOne(a => a.MedicalReport)
                .HasForeignKey<MedicalReport>(m => m.AppointmentId)
                .OnDelete(DeleteBehavior.NoAction);

            // ========== Unique Constraints ==========

            // منع تقييم المريض نفس الدكتور أكثر من مرة
            builder.Entity<Rating>()
                .HasIndex(r => new { r.DoctorId, r.PatientId })
                .IsUnique();

            // ========== Property Configurations ==========

            // Make DoctorId nullable في Patient table
            builder.Entity<Patient>()
                .Property(p => p.PrimaryDoctorId)
                .IsRequired(false); // nullable
        }
    }
}
