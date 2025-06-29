
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalConnect.Database
{
    public class DoctorAvailability
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [Display(Name = "Day of Week")]
        public DayOfWeek DayOfWeek { get; set; } // 0 = Sunday, 1 = Monday, etc.

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;

        // Navigation properties
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}