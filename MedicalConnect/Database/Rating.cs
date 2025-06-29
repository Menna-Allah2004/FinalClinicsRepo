
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalConnect.Database
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [Range(1, 5)]
        public int RatingValue { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
    }
}
