using System.ComponentModel.DataAnnotations;

namespace MedicalConnect.ViewModels
{
    public class PatientViewModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "مطلوب الاسم كامل")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "البريد الالكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "عنوان البريد الإلكتروني غير صالح")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }

        [Display(Name = "العنوان")]
        public string Address { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الميلاد")]
        public DateTime DateOfBirth { get; set; }

        public int Age { get; set; }

        [Display(Name = "التاريخ الطبي")]
        public string MedicalHistory { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "فصيلة الدم")]
        public string BloodType { get; set; }

        public DateTime? LastVisit { get; set; }

        [Display(Name = "صورة الملف الشخصي")]
        public string ImageUrl { get; set; }

        [Display(Name = "تحميل صورة الملف الشخصي")]
        public IFormFile ImageFile { get; set; }

        public List<AppointmentViewModel> UpcomingAppointments { get; set; }
        public List<AppointmentViewModel> PastAppointments { get; set; }
        public List<MedicalReportViewModel> MedicalReports { get; set; }
    }

    public class PatientProfileViewModel : PatientViewModel
    {
        public string UserId { get; set; }
        public int PatientId { get; set; }

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صالح")]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }

        [Display(Name = "العنوان")]
        public string Location { get; set; }

        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public int? Age { get; set; }

        [Display(Name = "المدينة")]
        public string City { get; set; }

        [Display(Name = "الجنس")]
        public string Gender { get; set; }

        [Display(Name = "فصيلة الدم")]
        public string? BloodType { get; set; }

        [Display(Name = "الطول (سم)")]
        [Range(0, 300, ErrorMessage = "الطول يجب أن يكون بين 0 و 300 سم")]
        public double? Height { get; set; }

        [Display(Name = "الوزن (كيلو)")]
        [Range(0, 500, ErrorMessage = "الوزن يجب أن يكون بين 0 و 500 كيلو")]
        public double? Weight { get; set; }

        [Display(Name = "الحالة الاجتماعية")]
        public string MaritalStatus { get; set; }

        [Display(Name = "صورة الملف الشخصي")]
        public string? ImageUrl { get; set; }

        public int TotalAppointments { get; set; }
        public int TotalMedicalReports { get; set; }
        public List<AppointmentViewModel> RecentAppointments { get; set; } = new();
        public List<MedicalReportViewModel> RecentMedicalReports { get; set; } = new();

        public List<AppointmentViewModel> UpcomingAppointments { get; set; }
        public List<AppointmentViewModel> PastAppointments { get; set; }
        public List<MedicalReportViewModel> MedicalReports { get; set; }
    }

    public class PatientListViewModel
    {
        public PatientViewModel[] Patients { get; set; } = new PatientViewModel[0];
        public int TotalPatientsCount { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchQuery { get; set; } = "";
    }

    public class PatientAppointmentsViewModel
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientCode => $"PAT-{PatientId:D5}";

        // Stats
        public int TotalAppointmentsCount { get; set; }
        public int CompletedAppointmentsCount { get; set; }
        public int UpcomingAppointmentsCount { get; set; }
        public int CancelledAppointmentsCount { get; set; }

        // Lists
        public IEnumerable<AppointmentViewModel> AllAppointments { get; set; } = new List<AppointmentViewModel>();
        public IEnumerable<AppointmentViewModel> UpcomingAppointments { get; set; } = new List<AppointmentViewModel>();
        public IEnumerable<AppointmentViewModel> CompletedAppointments { get; set; } = new List<AppointmentViewModel>();
        public IEnumerable<AppointmentViewModel> CancelledAppointments { get; set; } = new List<AppointmentViewModel>();
    }
}
