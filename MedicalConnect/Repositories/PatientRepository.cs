using MedicalConnect.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        private readonly ApplicationDbContext _db;

        public PatientRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Patient> GetPatientByUserIdAsync(string userId)
        {
            return await _db.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
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
}