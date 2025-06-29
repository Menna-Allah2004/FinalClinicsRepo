
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalConnect.Database
{
    public class MedicalReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int PatientId { get; set; }

        public int? AppointmentId { get; set; }

        [Display(Name = "عنوان التقرير")]
        public string Title { get; set; }

        [Display(Name = "نوع التقرير")]
        public string Type { get; set; }

        [Required]
        public string Diagnosis { get; set; }

        public string Treatment { get; set; }

        public string Prescription { get; set; }

        public string Notes { get; set; }

        public string FilePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; }

        public string ReportDetails { get; set; }
        public DateTime ReportDate { get; set; }
    }
}
