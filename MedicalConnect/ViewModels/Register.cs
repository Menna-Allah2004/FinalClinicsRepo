using System.ComponentModel.DataAnnotations;

namespace MedicalConnect.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        // إضافة حقل لنوع المستخدم (طبيب أو مريض)
        [Display(Name = "نوع المستخدم")]
        public string UserType { get; set; }

        [Display(Name = "تذكرني؟")]
        public bool RememberMe { get; set; }
    }

    public class RegisterPatientViewModel
    {
        [Required]
        [Display(Name = "User Type")]
        public string UserType { get; set; } = "Patient";

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, ErrorMessage = "يجب أن تكون كلمة المرور على الأقل {2} حرفًا.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
        public string ConfirmPassword { get; set; }

        [Phone(ErrorMessage = "رقم الهاتف غير صالح")]
        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        // حقول إضافية للمريض
        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "الجنس")]
        public string? Gender { get; set; }

        [Display(Name = "فصيلة الدم")]
        public string? BloodType { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        [Display(Name = "المدينة")]
        public string? City { get; set; }

        [Required(ErrorMessage = "يجب الموافقة على شروط الاستخدام وسياسة الخصوصية")]
        [Display(Name = "أوافق على شروط الاستخدام وسياسة الخصوصية")]
        public bool AgreeTerms { get; set; }

        // URL العودة بعد التسجيل
        [Display(Name = "Return URL")]
        public string? ReturnUrl { get; set; }
    }

    public class RegisterDoctorViewModel
    {
        [Required]
        [Display(Name = "User Type")]
        public string UserType { get; set; } = "Doctor";

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, ErrorMessage = "يجب أن تكون كلمة المرور على الأقل {2} حرفًا.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صالح")]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }

        [Display(Name = "الجنس")]
        public string Gender { get; set; }

        // Additional fields for Doctor
        [Required(ErrorMessage = "التخصص مطلوب")]
        [Display(Name = "التخصص")]
        public string Specialty { get; set; }

        [Display(Name = "السيرة الذاتية")]
        public string? Bio { get; set; }

        [Display(Name = "التعليم")]
        public string? Education { get; set; }

        [Display(Name = "سنوات الخبرة")]
        public int? ExperienceYears { get; set; }

        [Display(Name = "رسوم الاستشارة")]
        public decimal? ConsultationFee { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        [Display(Name = "المدينة")]
        public string City { get; set; }

        [Display(Name = "رقم الترخيص الطبي")]
        public string? License { get; set; }

        [Display(Name = "العيادة/المستشفى")]
        public string? Workplace { get; set; }

        [Display(Name = "عنوان العيادة")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "يجب الموافقة على شروط الاستخدام وسياسة الخصوصية")]
        [Display(Name = "أوافق على شروط الاستخدام وسياسة الخصوصية")]
        public bool AgreeTerms { get; set; }

        // URL العودة بعد التسجيل
        [Display(Name = "Return URL")]
        public string? ReturnUrl { get; set; }
    }
}
