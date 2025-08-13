using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalConnect.Database
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }

        [Required]
        [Display(Name = "Specialization")]
        [StringLength(100)]
        public string Specialty { get; set; }

        [StringLength(100)]
        public string SubSpecialty { get; set; }

        [Display(Name = "المؤهل العلمي")]
        public string? Education { get; set; }

        [Display(Name = "اسم المكان / العيادة / المستشفى")]
        public string? Workplace { get; set; }

        public string City { get; set; }

        [Required]
        [Range(1, 50)]
        [Display(Name = "سنوات الخبرة")]
        public int? Experience { get; set; }

        public string? License { get; set; }

        public string? Location { get; set; }

        [StringLength(2000)]
        public string? Bio { get; set; }

        [Display(Name = "Rating")]
        //[Range(0, 5)]
        [Column(TypeName = "decimal(3,1)")]
        public decimal? Rating { get; set; } = 0;

        public string? ImageUrl { get; set; }

        public int RatingCount { get; set; } = 0;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? ConsultationFee { get; set; }

        [Range(15, 120)]
        public int ConsultationDuration { get; set; } = 30; // in minutes

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public string? WorkingHours { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? BloodType { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool IsVerified { get; set; } = false;
        public bool AcceptingNewPatients { get; set; } = true;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
        public virtual ICollection<MedicalReport> MedicalReports { get; set; } = new HashSet<MedicalReport>();
        public virtual ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
        public virtual ICollection<DoctorAvailability> Availabilities { get; set; }
    }
}