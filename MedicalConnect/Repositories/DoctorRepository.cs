using MedicalConnect.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
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
}