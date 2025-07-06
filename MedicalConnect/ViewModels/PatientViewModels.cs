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

        [Display(Name = "الجنس")]
        public string Gender { get; set; }

        [Display(Name = "فصيلة الدم")]
        public string BloodType { get; set; }

        [Display(Name = "التاريخ الطبي")]
        public string MedicalHistory { get; set; }

        [Display(Name = "صورة الملف الشخصي")]
        public string ImageUrl { get; set; }

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
}
