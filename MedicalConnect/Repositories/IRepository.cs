using MedicalConnect.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IPatientRepository Patients { get; }
        IDoctorRepository Doctors { get; }
        IAppointmentRepository Appointments { get; }
        IMedicalReportRepository MedicalReports { get; }
        IDoctorAvailabilityRepository DoctorAvailabilities { get; }
        IContactUsMessageRepository ContactUsMessages { get; }
        IRatingRepository Ratings { get; }
        INotificationRepository Notifications { get; }

        ApplicationDbContext Context { get; }

        Task<int> SaveAsync();
        void Save();
    }

    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null);
        Task<T> GetByIdAsync(int id);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null,
            string includeProperties = null);
        Task AddAsync(T entity);
        Task RemoveAsync(int id);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
    }

    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetPastAppointmentsAsync(int doctorId);
        Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan time);
    }

    public interface IContactUsMessageRepository : IRepository<ContactUsMessage>
    {
        Task<IEnumerable<ContactUsMessage>> GetUnreadMessagesAsync();
        Task<IEnumerable<ContactUsMessage>> GetMessagesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }

    public interface IDoctorAvailabilityRepository : IRepository<DoctorAvailability>
    {
        Task<IEnumerable<DoctorAvailability>> GetAvailabilitiesByDoctorIdAsync(int doctorId);
        Task<DoctorAvailability> GetAvailabilityByDoctorAndDayAsync(int doctorId, string dayOfWeek);
    }

    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization);
        Task<IEnumerable<Doctor>> SearchDoctorsAsync(string searchTerm);
        Task UpdateRatingAsync(int doctorId, decimal newRating);
    }

    public interface IMedicalReportRepository : IRepository<MedicalReport>
    {
        Task<IEnumerable<MedicalReport>> GetReportsByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicalReport>> GetReportsByDoctorIdAsync(int doctorId);
        Task<MedicalReport> GetReportByAppointmentIdAsync(int appointmentId);
        Task<IEnumerable<MedicalReport>> GetReportsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }

    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsByUserIdAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
    }

    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient> GetPatientByUserIdAsync(string userId);
        void Update(Patient patient);
        Task UpdateMedicalHistoryAsync(int patientId, string medicalHistory);
        Task<IEnumerable<Patient>> GetAllWithDetailsAsync();
        Task<Patient> GetWithDetailsAsync(int id);
    }

    public interface IRatingRepository : IRepository<Rating>
    {
        Task<IEnumerable<Rating>> GetRatingsByDoctorIdAsync(int doctorId);
        Task<double> GetAverageRatingByDoctorIdAsync(int doctorId);
        Task<Rating> GetRatingByPatientAndDoctorAsync(int patientId, int doctorId);
    }
}