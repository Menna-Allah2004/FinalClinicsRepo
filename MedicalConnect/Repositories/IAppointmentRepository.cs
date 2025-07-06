using MedicalConnect.Database;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime appointmentTime);
        Task UpdateStatusAsync(int appointmentId, string status);
        Task<int> CountAsync(Expression<Func<Appointment, bool>> filter);
    }
}