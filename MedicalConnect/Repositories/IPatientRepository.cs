using MedicalConnect.Database;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient> GetPatientByUserIdAsync(string userId);
        Task UpdateMedicalHistoryAsync(int patientId, string medicalHistory);
    }
}