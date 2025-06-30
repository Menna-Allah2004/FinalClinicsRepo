using MedicalConnect.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public class DoctorAvailabilityRepository : Repository<DoctorAvailability>, IDoctorAvailabilityRepository
    {
        private readonly ApplicationDbContext _db;

        public DoctorAvailabilityRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<DoctorAvailability>> GetAvailabilityByDoctorIdAsync(int doctorId)
        {
            return await _db.DoctorAvailabilities
                .Where(da => da.DoctorId == doctorId)
                .OrderBy(da => da.DayOfWeek)
                .ThenBy(da => da.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<DateTime>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
        {
            // Get the doctor's availability for the given day of week
            var dayOfWeek = date.DayOfWeek;
            var availabilities = await _db.DoctorAvailabilities
                .Where(da => da.DoctorId == doctorId && da.DayOfWeek == dayOfWeek)
                .ToListAsync();

            if (!availabilities.Any())
                return new List<DateTime>();

            // Get existing appointments for that day
            var existingAppointments = await _db.Appointments
                .Where(a => a.DoctorId == doctorId &&
                           a.AppointmentDate.Date == date.Date &&
                           a.Status != "Cancelled")
                .Select(a => a.AppointmentDate.TimeOfDay)
                .ToListAsync();

            // Generate available time slots (assuming 30-minute appointments)
            var availableSlots = new List<DateTime>();
            foreach (var availability in availabilities)
            {
                var currentTime = availability.StartTime;
                var endTime = availability.EndTime;

                while (currentTime.Add(TimeSpan.FromMinutes(30)) <= endTime)
                {
                    if (!existingAppointments.Contains(currentTime))
                    {
                        availableSlots.Add(date.Date.Add(currentTime));
                    }
                    currentTime = currentTime.Add(TimeSpan.FromMinutes(30));
                }
            }

            return availableSlots.OrderBy(dt => dt);
        }
    }
}