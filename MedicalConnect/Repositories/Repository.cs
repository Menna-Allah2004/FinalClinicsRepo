using MedicalConnect.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IPatientRepository _patients;
        private IDoctorRepository _doctors;
        private IAppointmentRepository _appointments;
        private IMedicalReportRepository _medicalReports;
        private IRatingRepository _ratings;
        private INotificationRepository _notifications;
        private IDoctorAvailabilityRepository _doctorAvailabilities;
        private IContactUsMessageRepository _contactUsMessages;

        public ApplicationDbContext Context => _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IPatientRepository Patients
        {
            get
            {
                if (_patients == null)
                    _patients = new PatientRepository(_context);
                return _patients;
            }
        }

        public IDoctorRepository Doctors
        {
            get
            {
                if (_doctors == null)
                    _doctors = new DoctorRepository(_context);
                return _doctors;
            }
        }

        public IAppointmentRepository Appointments
        {
            get
            {
                if (_appointments == null)
                    _appointments = new AppointmentRepository(_context);
                return _appointments;
            }
        }

        public IMedicalReportRepository MedicalReports
        {
            get
            {
                if (_medicalReports == null)
                    _medicalReports = new MedicalReportRepository(_context);
                return _medicalReports;
            }
        }

        public IRatingRepository Ratings
        {
            get
            {
                if (_ratings == null)
                    _ratings = new RatingRepository(_context);
                return _ratings;
            }
        }

        public INotificationRepository Notifications
        {
            get
            {
                if (_notifications == null)
                    _notifications = new NotificationRepository(_context);
                return _notifications;
            }
        }

        public IDoctorAvailabilityRepository DoctorAvailabilities
        {
            get
            {
                if (_doctorAvailabilities == null)
                    _doctorAvailabilities = new DoctorAvailabilityRepository(_context);
                return _doctorAvailabilities;
            }
        }

        public IContactUsMessageRepository ContactUsMessages
        {
            get
            {
                if (_contactUsMessages == null)
                    _contactUsMessages = new ContactUsMessageRepository(_context);
                return _contactUsMessages;
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null,
            string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(int id)
        {
            T entity = await dbSet.FindAsync(id);
            await RemoveAsync(entity);
        }

        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _db.Update(entity);
            await _db.SaveChangesAsync();
        }
    }

    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.AppointmentDate.Date == date.Date)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int doctorId)
        {
            var today = DateTime.Today;
            return await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctorId &&
                           a.AppointmentDate > today &&
                           a.Status != "Cancelled")
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync(int doctorId)
        {
            var today = DateTime.Today;
            return await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctorId &&
                           a.AppointmentDate.Date == today &&
                           a.Status != "Cancelled")
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetPastAppointmentsAsync(int doctorId)
        {
            var today = DateTime.Today;
            return await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctorId &&
                           a.AppointmentDate < today)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan time)
        {
            return !await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId &&
                              a.AppointmentDate.Date == date.Date &&
                              a.AppointmentTime == time &&
                              a.Status != "Cancelled");
        }
    }

    public class ContactUsMessageRepository : Repository<ContactUsMessage>, IContactUsMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactUsMessageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContactUsMessage>> GetUnreadMessagesAsync()
        {
            return await _context.ContactUsMessages
                .Where(m => !m.IsRead)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContactUsMessage>> GetMessagesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.ContactUsMessages
                .Where(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
    }

    public class DoctorAvailabilityRepository : Repository<DoctorAvailability>, IDoctorAvailabilityRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorAvailabilityRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorAvailability>> GetAvailabilitiesByDoctorIdAsync(int doctorId)
        {
            return await _context.DoctorAvailabilities
                .Where(da => da.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<DoctorAvailability> GetAvailabilityByDoctorAndDayAsync(int doctorId, string dayOfWeek)
        {
            return await _context.DoctorAvailabilities
                .FirstOrDefaultAsync(da => da.DoctorId == doctorId && da.DayOfWeek.ToString() == dayOfWeek);
        }
    }

    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        private readonly ApplicationDbContext _db;

        public DoctorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization)
        {
            return await _db.Doctors
                .Include(d => d.User)
                .Where(d => d.Specialty.Contains(specialization))
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> SearchDoctorsAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await GetAllAsync();

            return await _db.Doctors
                .Include(d => d.User)
                .Where(d => d.User.FullName.Contains(searchTerm) ||
                            d.Specialty.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task UpdateRatingAsync(int doctorId, decimal newRating)
        {
            var doctor = await _db.Doctors.FindAsync(doctorId);
            if (doctor != null)
            {
                doctor.Rating = newRating;
                await _db.SaveChangesAsync();
            }
        }
    }

    public class MedicalReportRepository : Repository<MedicalReport>, IMedicalReportRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicalReportRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicalReport>> GetReportsByPatientIdAsync(int patientId)
        {
            return await _context.MedicalReports
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Appointment)
                .Where(mr => mr.PatientId == patientId)
                .OrderByDescending(mr => mr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalReport>> GetReportsByDoctorIdAsync(int doctorId)
        {
            return await _context.MedicalReports
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Appointment)
                .Where(mr => mr.DoctorId == doctorId)
                .OrderByDescending(mr => mr.CreatedAt)
                .ToListAsync();
        }

        public async Task<MedicalReport> GetReportByAppointmentIdAsync(int appointmentId)
        {
            return await _context.MedicalReports
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Appointment)
                .FirstOrDefaultAsync(mr => mr.AppointmentId == appointmentId);
        }

        public async Task<IEnumerable<MedicalReport>> GetReportsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.MedicalReports
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Appointment)
                .Where(mr => mr.CreatedAt >= startDate && mr.CreatedAt <= endDate)
                .OrderByDescending(mr => mr.CreatedAt)
                .ToListAsync();
        }
    }

    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }

    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        private readonly ApplicationDbContext _db;

        public PatientRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Patient>> GetAllWithDetailsAsync()
        {
            return await _db.Patients
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<Patient> GetWithDetailsAsync(int id)
        {
            return await _db.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Patient> GetPatientByUserIdAsync(string userId)
        {
            return await _db.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public void Update(Patient patient)
        {
            _db.Patients.Update(patient);
        }

        public async Task UpdateMedicalHistoryAsync(int patientId, string medicalHistory)
        {
            var patient = await _db.Patients.FindAsync(patientId);
            if (patient != null)
            {
                patient.MedicalHistory = medicalHistory;
                await _db.SaveChangesAsync();
            }
        }
    }

    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public RatingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rating>> GetRatingsByDoctorIdAsync(int doctorId)
        {
            return await _context.Ratings
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .Where(r => r.DoctorId == doctorId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingByDoctorIdAsync(int doctorId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.DoctorId == doctorId)
                .ToListAsync();

            return ratings.Any() ? ratings.Average(r => r.RatingValue) : 0;
        }

        public async Task<Rating> GetRatingByPatientAndDoctorAsync(int patientId, int doctorId)
        {
            return await _context.Ratings
                .FirstOrDefaultAsync(r => r.PatientId == patientId && r.DoctorId == doctorId);
        }
    }
}