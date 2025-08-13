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

        public bool IsReplied { get; set; } = false;

        [StringLength(2000)]
        public string AdminReply { get; set; }

        public DateTime? RepliedAt { get; set; }

        [StringLength(100)]
        public string RepliedBy { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "New"; // New, InProgress, Resolved, Closed

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [StringLength(50)]
        public string MessageType { get; set; } = "General"; // General, Complaint, Suggestion, Question

        [DataType(DataType.DateTime)]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}