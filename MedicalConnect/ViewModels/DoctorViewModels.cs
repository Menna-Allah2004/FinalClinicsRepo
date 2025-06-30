using MedicalConnect.Database;
using System.ComponentModel.DataAnnotations;

namespace MedicalConnect.ViewModels
{
    public class DoctorViewModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "التخصص مطلوب")]
        [Display(Name = "التخصص")]
        public string Specialty { get; set; }

        [Display(Name = "نبذة شخصية")]
        public string Bio { get; set; }

        [Display(Name = "المؤهلات العلمية")]
        public string Education { get; set; }

        [Required(ErrorMessage = "سنوات الخبرة مطلوبة")]
        [Display(Name = "سنوات الخبرة")]
        public int? ExperienceYears { get; set; }

        [Display(Name = "التقييم")]
        public decimal Rating { get; set; }

        [Display(Name = "المدينة")]
        public string City { get; set; }

        [Display(Name = "عدد التقييمات")]
        public int RatingCount { get; set; }

        [Display(Name = "رسوم الاستشارة")]
        [DataType(DataType.Currency)]
        public decimal? ConsultationFee { get; set; }

        [Display(Name = "صورة الملف الشخصي")]
        public string ImageUrl { get; set; }

        [Phone(ErrorMessage = "رقم الهاتف مطلوب")]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Display(Name = "تحميل صورة الملف الشخصي")]
        public IFormFile ProfileImage { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        public List<DoctorAvailability> Availabilities { get; set; }

        public List<AppointmentViewModel> Appointments { get; set; }
    }

    public class DoctorsListViewModel
    {
        public List<DoctorViewModel> Doctors { get; set; }
        public string SearchTerm { get; set; }
        public string Specialization { get; set; }
        public List<string> Specializations { get; set; }
    }

    public class DoctorSearchViewModel
    {
        public List<DoctorViewModel> DoctorsView { get; set; }
        public string? SearchQuery { get; set; }
        public string? Specialty { get; set; }
        public string? Location { get; set; }
        public string? Rating { get; set; }
        public string? Price { get; set; }
        public bool AvailableToday { get; set; }
        public bool NoResults => Doctors == null || Doctors.Count == 0;
        public int DoctorCount => Doctors?.Count ?? 0;

        // قائمة الأطباء المعروضة
        public List<DoctorViewModel> Doctors { get; set; } = new();

        // 🔽 الخصائص الجديدة التي تحتاجينها حسب الرسائل:
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? SortBy { get; set; }
        public string? SelectedSpecialty { get; set; }
        public string? SelectedLocation { get; set; }

        // في حال تحتاجين عرض قائمة بالاختيارات للموقع:
        public List<string>? Locations { get; set; }
        public List<string> Specialties { get; set; } = new List<string>();
    }

    public class DoctorDashboardViewModel
    {
        public string DoctorName { get; set; }
        public string Greeting { get; set; }
        public DateTime CurrentDate { get; set; }
        public int PatientCount { get; set; }
        public int ConsultationCount { get; set; }
        public int CompletedTasksPercentage { get; set; }
        public List<AppointmentViewModel> TodayAppointments { get; set; }
        public List<AppointmentViewModel> UpcomingAppointments { get; set; }
        public List<AppointmentViewModel> CompletedAppointments { get; set; }
        public List<PatientViewModel> Patients { get; set; }
    }
}
