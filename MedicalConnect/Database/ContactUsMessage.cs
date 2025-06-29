using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalConnect.Database
{
    public class ContactUsMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; }

        public bool IsRead { get; set; } = false;

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}