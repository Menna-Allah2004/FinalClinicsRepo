
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace MedicalConnect.Database
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointment Date and Time")]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [Range(15, 120)]
        public int Duration { get; set; } = 30;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Display(Name = "Status")]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string Title { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = "InPerson";

        [StringLength(500)]
        public string Reason { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal? Fee { get; set; }

        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending";

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        [StringLength(1000)]
        public string CancellationReason { get; set; }

        public DateTime? CancellationDate { get; set; }

        public bool IsVirtual { get; set; }

        public string? MeetingLink { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        public virtual MedicalReport MedicalReport { get; set; }
    }
}