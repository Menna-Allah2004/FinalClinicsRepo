using System.Collections.Generic;

namespace MedicalConnect.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int PendingDoctorApprovals { get; set; }
        public int UnreadMessages { get; set; }
        public List<DoctorApprovalViewModel> PendingDoctors { get; set; }
        public List<ContactUsMessageViewModel> RecentMessages { get; set; }
    }

    public class DoctorApprovalViewModel
    {
        public string UserId { get; set; }
        public int DoctorId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public string ImageUrl { get; set; }
        public int? ExperienceYears { get; set; }
    }

    //public class ContactUsMessageViewModel
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Email { get; set; }
    //    public string Subject { get; set; }
    //    public string Message { get; set; }
    //    public bool IsRead { get; set; }
    //    public string CreatedAt { get; set; }
    //}

    public class MonthlyVisitData
    {
        public string Month { get; set; }
        public int VisitCount { get; set; }
    }
}
