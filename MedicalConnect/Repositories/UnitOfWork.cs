using MedicalConnect.Database;
using System;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IPatientRepository _patientRepository;
        private IDoctorRepository _doctorRepository;
        private IAppointmentRepository _appointmentRepository;
        private IMedicalReportRepository _medicalReportRepository;
        private IDoctorAvailabilityRepository _doctorAvailabilityRepository;
        private IContactUsMessageRepository _contactUsMessageRepository;
        private IRatingRepository _ratingRepository;
        private INotificationRepository _notificationRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IPatientRepository Patients => _patientRepository ??= new PatientRepository(_context);
        public IDoctorRepository Doctors => _doctorRepository ??= new DoctorRepository(_context);
        public IAppointmentRepository Appointments => _appointmentRepository ??= new AppointmentRepository(_context);
        public IMedicalReportRepository MedicalReports => _medicalReportRepository ??= new MedicalReportRepository(_context);
        public IDoctorAvailabilityRepository DoctorAvailabilities => _doctorAvailabilityRepository ??= new DoctorAvailabilityRepository(_context);
        public IContactUsMessageRepository ContactUsMessages => _contactUsMessageRepository ??= new ContactUsMessageRepository(_context);
        public IRatingRepository Ratings => _ratingRepository ??= new RatingRepository(_context);
        public INotificationRepository Notifications => _notificationRepository ??= new NotificationRepository(_context);

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}