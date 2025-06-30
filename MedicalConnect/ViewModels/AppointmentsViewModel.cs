using MedicalConnect.Database;
using System.ComponentModel.DataAnnotations;

namespace MedicalConnect.ViewModels
{
    public class AppointmentViewModel
    {
        public Patient Patient { get; set; } // استدعاء موديل المريض
        public int Id { get; set; }

        [Required(ErrorMessage = "الطبيب مطلوب")]
        [Display(Name = "الطبيب")]
        public int DoctorId { get; set; }

        public string DoctorName { get; set; }
        public string DoctorSpecialty { get; set; }
        public string DoctorImageUrl { get; set; }

        [Required(ErrorMessage = "المريض مطلوب")]
        [Display(Name = "المريض")]
        public int PatientId { get; set; }

        public string PatientName { get; set; }

        [Required(ErrorMessage = "تاريخ الموعد مطلوب")]
        [Display(Name = "تاريخ الموعد")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "وقت بدء الموعد مطلوب")]
        [Display(Name = "وقت بدء الموعد")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "وقت انتهاء الموعد مطلوب")]
        [Display(Name = "وقت انتهاء الموعد")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "حالة الموعد")]
        public string Status { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class BookAppointmentViewModel
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public string DoctorImageUrl { get; set; }
        public decimal? ConsultationFee { get; set; }
        public List<DateTime> AvailableDates { get; set; }
        public List<TimeSlotViewModel> AvailableTimeSlots { get; set; }
        public AppointmentViewModel Appointment { get; set; }
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
        public PatientViewModel Patients { get; set; }      // استدعاء موديل المريض
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
}
