using System.Collections.Generic;
using System.Threading.Tasks;
using MedicalConnect.Database;

namespace MedicalConnect.Repositories
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetAllAsync();
        Task<Rating> GetByIdAsync(int id);
        Task AddAsync(Rating rating);
        Task UpdateAsync(Rating rating);
        Task DeleteAsync(int id);
    }
}
