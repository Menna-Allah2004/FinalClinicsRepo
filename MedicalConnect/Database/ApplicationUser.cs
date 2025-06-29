using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicalConnect.Database
{
    public class ApplicationUser : IdentityUser, IValidatableObject
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        public string? Location { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public string UserType { get; set; } // "Admin", "Doctor", or "Patient"

        // الحالة: هل الآدمن وافق على حساب الطبيب؟
        public bool IsApproved { get; set; } = false; // الافتراضي غير موافق عليه

        public DateTime? BirthDate { get; set; }

        public string? Gender { get; set; }

        public string? City { get; set; }

        // Doctor specific fields
        public string? Specialty { get; set; }
        public int? Experience { get; set; }
        public string? License { get; set; }
        public string? Workplace { get; set; }
        public string? Bio { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Doctor Doctor { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        // Custom validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserType == "Doctor")
            {
                if (string.IsNullOrWhiteSpace(Specialty))
                    yield return new ValidationResult("Specialty is required for doctors.", new[] { nameof(Specialty) });

                if (!Experience.HasValue)
                    yield return new ValidationResult("Experience is required for doctors.", new[] { nameof(Experience) });

                if (string.IsNullOrWhiteSpace(License))
                    yield return new ValidationResult("License is required for doctors.", new[] { nameof(License) });

                if (string.IsNullOrWhiteSpace(Workplace))
                    yield return new ValidationResult("Workplace is required for doctors.", new[] { nameof(Workplace) });

                if (string.IsNullOrWhiteSpace(Bio))
                    yield return new ValidationResult("Bio is required for doctors.", new[] { nameof(Bio) });
            }
        }
    }
}
