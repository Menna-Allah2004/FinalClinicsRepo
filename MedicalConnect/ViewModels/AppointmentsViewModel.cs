using MedicalConnect.Database;
using System.ComponentModel.DataAnnotations;

namespace MedicalConnect.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الطبيب مطلوب")]
        [Display(Name = "الطبيب")]
        public int DoctorId { get; set; }

        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public string DoctorImageUrl { get; set; }

        public string DoctorLocation { get; set; }

        [Required(ErrorMessage = "المريض مطلوب")]
        [Display(Name = "المريض")]
        public int PatientId { get; set; }

        public string PatientName { get; set; }

        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "تاريخ ووقت الموعد مطلوب")]
        [Display(Name = "تاريخ ووقت الموعد")]
        [DataType(DataType.DateTime)]
        public TimeSpan AppointmentTime { get; set; }

        [Required(ErrorMessage = "وقت بدء الموعد مطلوب")]
        [Display(Name = "وقت بدء الموعد")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "وقت انتهاء الموعد مطلوب")]
        [Display(Name = "وقت انتهاء الموعد")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        // المدة المتوقعة للموعد بالدقائق
        public int DurationMinutes => (int)EndTime.Subtract(StartTime).TotalMinutes;

        [Display(Name = "عنوان الموعد")]
        [Required(ErrorMessage = "عنوان الموعد مطلوب")]
        public string Title { get; set; }

        [Display(Name = "نوع الموعد")]
        [Required(ErrorMessage = "نوع الموعد مطلوب")]
        public string Type { get; set; } // استشارة، فحص، إلخ

        [Display(Name = "حالة الموعد")]
        public string Status { get; set; } // مؤكد، ملغي، منتظر، مكتمل

        public string Reason { get; set; }

        public decimal? Fee { get; set; }

        public string PaymentStatus { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // معلومات التواصل الافتراضي
        [Display(Name = "رابط الاجتماع عبر الإنترنت")]
        public string MeetingLink { get; set; }

        // تحديد إذا كان الموعد عبر الإنترنت أو حضوري
        [Display(Name = "موعد افتراضي")]
        public bool IsVirtual { get; set; }

        public bool CanCancel { get; set; }
        public bool CanReschedule { get; set; }

        public string FormattedDate => AppointmentDate.ToString("dddd، dd MMMM yyyy", new System.Globalization.CultureInfo("ar-EG"));
        public string FormattedTime => AppointmentTime.ToString(@"hh\:mm") + (AppointmentTime.Hours < 12 ? " صباحاً" : " مساءً");
        public string StatusDisplay => Status switch
        {
            "Scheduled" => "مجدول",
            "Completed" => "مكتمل",
            "Cancelled" => "ملغي",
            "InProgress" => "جاري",
            _ => Status
        };
        public string TypeDisplay => IsVirtual ? "استشارة عن بعد" : "زيارة في العيادة";
        public string PaymentStatusDisplay => PaymentStatus switch
        {
            "Paid" => "مدفوع",
            "Pending" => "معلق",
            "Refunded" => "مسترد",
            _ => PaymentStatus
        };

    }

    public class BookAppointmentViewModel
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public string DoctorLocation { get; set; }
        public decimal? DoctorRating { get; set; }
        public int DoctorReviews { get; set; }
        public string DoctorImageUrl { get; set; }
        public decimal? ConsultationFee { get; set; }
        public List<DateTime> AvailableDates { get; set; } = new List<DateTime>();
        public List<TimeSlotViewModel> AvailableTimeSlots { get; set; }
        public AppointmentViewModel Appointment { get; set; }

        [Required(ErrorMessage = "نوع الموعد مطلوب")]
        public string AppointmentType { get; set; }

        [Required(ErrorMessage = "سبب الزيارة مطلوب")]
        public string Reason { get; set; }

        public string Notes { get; set; }

        public string PatientName { get; set; }
        public string PatientPhone { get; set; }
        public string PatientEmail { get; set; }

        public DateTime SelectedDate { get; set; }
        public TimeSpan SelectedTime { get; set; }

        public string PaymentMethod { get; set; }

        // For confirmation step
        public string BookingNumber { get; set; }
        public bool IsConfirmed { get; set; }

        public Doctor Doctor { get; set; } // معلومات الطبيب للحجز
    }

    public class AvailableTimesRequest
    {
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
    }

    public class TimeSlotViewModel
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }

        // عرض وقت البداية والنهاية بتنسيق مقروء
        public string FormattedTimeSlot => $"{StartTime.ToString(@"hh\:mm")} - {EndTime.ToString(@"hh\:mm")}";
    }

    public class AppointmentListViewModel
    {
        public PatientViewModel Patients { get; set; }
        public AppointmentViewModel[] UpcomingAppointments { get; set; }
        public AppointmentViewModel[] PastAppointments { get; set; }
        public AppointmentViewModel[] TodayAppointments { get; set; }

        public int TotalAppointmentsCount { get; set; }
        public int UpcomingAppointmentsCount { get; set; }
        public int TodayAppointmentsCount { get; set; }
        public int CancelledAppointmentsCount { get; set; }

        // للتحقق من وجود مواعيد
        public bool HasAnyAppointments => TotalAppointmentsCount > 0;

        public CalendarDayViewModel CalendarDayViewModel { get; set; }
    }

    public class CalendarDayViewModel
    {
        public AppointmentListViewModel Appointments { get; set; }
        public DateTime Date { get; set; }
        public int AppointmentsCount { get; set; }
        public bool IsToday { get; set; }
        public bool HasAppointments { get; set; }
        public int Day { get; set; }
    }

    public class CancelAppointmentRequest
    {
        public string Reason { get; set; }
        public string OtherReason { get; set; }
    }

    public class RescheduleAppointmentRequest
    {
        public DateTime NewDate { get; set; }
        public TimeSpan NewTime { get; set; }
    }
}
