using MedicalConnect.Database;
using System.ComponentModel.DataAnnotations;

namespace MedicalConnect.ViewModels
{
    public class ReportViewModel
    {
        public int Id { get; set; }

        [Display(Name = "عنوان التقرير")]
        public string Title { get; set; }

        [Display(Name = "نوع التقرير")]
        public string Type { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "حالة التقرير")]
        public string Status { get; set; }

        [Display(Name = "رابط التحميل")]
        public string DownloadUrl { get; set; }

        [Display(Name = "حجم الملف")]
        public string FileSize { get; set; }

        [Display(Name = "وصف التقرير")]
        public string Description { get; set; }
    }

    public class DoctorReportsViewModel
    {
        [Display(Name = "إجمالي الزيارات")]
        public int TotalVisits { get; set; }

        [Display(Name = "الزيارات هذا الشهر")]
        public int MonthlyVisits { get; set; }

        [Display(Name = "الزيارات هذا الأسبوع")]
        public int WeeklyVisits { get; set; }

        [Display(Name = "متوسط الزيارات اليومية")]
        public double DailyAverage { get; set; }

        [Display(Name = "التقارير الأخيرة")]
        public List<MedicalReport> RecentReports { get; set; }
        public bool HasReports => RecentReports != null && RecentReports.Count > 0;

        [Display(Name = "بيانات الرسم البياني")]
        public ChartDataViewModel ChartData { get; set; }

        public List<MonthlyVisitData> MonthlyVisitData { get; set; }

        public DoctorReportsViewModel()
        {
            RecentReports = new List<MedicalReport>();
            ChartData = new ChartDataViewModel();
        }

        public ReportViewModel ReportModel { get; set; }
    }

    public class ChartDataViewModel
    {
        public List<string> Labels { get; set; }
        public List<int> Data { get; set; }
        public string Title { get; set; } = "";
        public string ChartType { get; set; }

        public ChartDataViewModel()
        {
            Labels = new List<string>();
            Data = new List<int>();
            ChartType = "line";
        }
    }

    public class MedicalReportViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الطبيب مطلوب")]
        [Display(Name = "الطبيب")]
        public int DoctorId { get; set; }

        public string DoctorName { get; set; }

        [Required(ErrorMessage = "المريض مطلوب")]
        [Display(Name = "المريض")]
        public int PatientId { get; set; }

        public string PatientName { get; set; }

        [Display(Name = "موعد")]
        public int? AppointmentId { get; set; }

        public DateTime? AppointmentDate { get; set; }

        [Required(ErrorMessage = "التشخيص مطلوب")]
        [Display(Name = "التشخيص")]
        public string Diagnosis { get; set; }

        [Display(Name = "العلاج")]
        public string Treatment { get; set; }

        [Display(Name = "الوصفة الطبية")]
        public string Prescription { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
