using MedicalConnect.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        private readonly ApplicationDbContext _db;

        public AppointmentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            return await _db.Appointments
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
            return await _db.Appointments
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
            return await _db.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.AppointmentDate.Date == date.Date)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime appointmentTime)
        {
            // Check if the doctor has any appointments at the same time
            bool hasConflictingAppointment = await _db.Appointments
                .AnyAsync(a => a.DoctorId == doctorId &&
                               a.AppointmentDate == appointmentTime &&
                               a.Status != "Cancelled");

            if (hasConflictingAppointment)
                return false;

            // Check if the time is within the doctor's available slots
            var dayOfWeek = appointmentTime.DayOfWeek;
            var timeOfDay = appointmentTime.TimeOfDay;

            bool isWithinAvailableSlot = await _db.DoctorAvailabilities
                .AnyAsync(da => da.DoctorId == doctorId &&
                                da.DayOfWeek == dayOfWeek &&
                                da.StartTime <= timeOfDay &&
                                da.EndTime >= timeOfDay);

            return isWithinAvailableSlot;
        }

        public async Task UpdateStatusAsync(int appointmentId, string status)
        {
            var appointment = await _db.Appointments.FindAsync(appointmentId);
            if (appointment != null)
            {
                appointment.Status = status;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<int> CountAsync(Expression<Func<Appointment, bool>> filter)
        {
            return await _db.Appointments.CountAsync(filter);
        }
    }
}