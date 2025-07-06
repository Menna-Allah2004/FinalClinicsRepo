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

        public bool HasAvailability { get; set; }

        public string DisplayWaitingTime { get; set; }

        public List<DoctorAvailability> Availabilities { get; set; }

        public List<AppointmentViewModel> Appointments { get; set; }

        public DoctorDashboardViewModel DoctorsDash { get; set; }
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
        public DoctorViewModel Doctor { get; set; }
        public string? WorkingHours { get; set; }
        public int ScheduledMeetings { get; set; }
        public int ScheduledLabTests { get; set; }
        public int ScheduledConsultations { get; set; }
        public int InPatientsCountChange { get; set; }
        public int InPatientsCount { get; set; }
        public string DoctorLocation { get; set; }
        public string DoctorBloodType { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CurrentCalendarDate { get; set; }
        public int CompletedMeetings { get; set; }
        public int CompletedLabTests { get; set; }
        public int CompletedConsultations { get; set; }
        public IEnumerable<CalendarDayViewModel> CalendarDays { get; set; }
        public List<PatientListViewModel> Patients { get; set; }

        // إحصائيات الطبيب
        public string DoctorName { get; set; }
        public int PatientsCount { get; set; }
        public int OnlineConsultationsCount { get; set; }
        public int LabTestsCount { get; set; }

        // معلومات التقدم في المهام
        public int ConsultationsCount { get; set; }
        public int ConsultationsCompleted { get; set; }
        public int LabAnalysisCount { get; set; }
        public int LabAnalysisCompleted { get; set; }
        public int MeetingsCount { get; set; }
        public int MeetingsCompleted { get; set; }

        // معلومات المواعيد
        public List<AppointmentViewModel> TodayAppointments { get; set; }
        public List<AppointmentViewModel> UpcomingAppointments { get; set; }
        public List<AppointmentViewModel> CompletedAppointments { get; set; }

        // التاريخ الحالي
        public DateTime CurrentDate { get; set; }
        public string Greeting { get; set; }

        // حساب النسب المئوية للمهام
        public int ConsultationsPercentage => ConsultationsCount > 0 ? (int)(ConsultationsCompleted * 100 / ConsultationsCount) : 0;
        public int LabAnalysisPercentage => LabAnalysisCount > 0 ? (int)(LabAnalysisCompleted * 100 / LabAnalysisCount) : 0;
        public int MeetingsPercentage => MeetingsCount > 0 ? (int)(MeetingsCompleted * 100 / MeetingsCount) : 0;

        // النسبة المئوية الإجمالية للأحداث المجدولة
        public int TotalEventsPercentage
        {
            get
            {
                int total = ConsultationsCount + LabAnalysisCount + MeetingsCount;
                int completed = ConsultationsCompleted + LabAnalysisCompleted + MeetingsCompleted;
                return total > 0 ? (int)(completed * 100 / total) : 0;
            }
        }

        // نسبة إكمال المهام اليومية
        public int CompletedTasksPercentage { get; set; }

        // اتجاهات الإحصائيات (زيادة أو نقصان)
        public int PatientsCountChange { get; set; } // نسبة التغيير عن الأسبوع الماضي
        public int OnlineConsultationsChange { get; set; }
        public int LabTestsChange { get; set; }

        public class PatientDetailsViewModel
        {
            public PatientViewModel Patient { get; set; }
            public List<AppointmentViewModel> Appointments { get; set; }
        }
    }

    public class DoctorPatientsViewModel
    {
        public int TotalPatientsCount { get; set; }
        public List<PatientViewModel> Patients { get; set; } 
        public bool HasPatients => Patients != null && Patients.Count > 0;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchQuery { get; set; } = "";
    }

    public class DoctorSettingsViewModel
    {
        public DoctorViewModel ProfileSettings { get; set; }
        public NotificationSettingsViewModel NotificationSettings { get; set; }
        public SecuritySettingsViewModel SecuritySettings { get; set; }
    }

    public class NotificationSettingsViewModel
    {
        [Display(Name = "إشعارات المواعيد")]
        public bool AppointmentNotifications { get; set; }

        [Display(Name = "تذكيرات المهام")]
        public bool TaskReminders { get; set; }

        [Display(Name = "إشعارات البريد الإلكتروني")]
        public bool EmailNotifications { get; set; }
    }

    public class SecuritySettingsViewModel
    {
        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الحالية")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [StringLength(100, ErrorMessage = "يجب أن تكون كلمة المرور على الأقل {2} حرف", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الجديدة")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور الجديدة")]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور الجديدة وتأكيدها غير متطابقين")]
        public string ConfirmPassword { get; set; }
    }
}
