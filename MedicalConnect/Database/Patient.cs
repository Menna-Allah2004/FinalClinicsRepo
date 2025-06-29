
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalConnect.Database
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public string FullName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(5)]
        public string? BloodType { get; set; }

        [Display(Name = "Medical History")]
        public string? MedicalHistory { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int? Age { get; set; }

        public string? Condition { get; set; }

        public int? PrimaryDoctorId { get; set; }

        public DateTime? LastVisit { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<MedicalReport> MedicalReports { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}