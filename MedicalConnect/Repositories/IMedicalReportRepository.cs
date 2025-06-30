using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MedicalConnect.Database;

namespace MedicalConnect.Repositories
{
    public interface IMedicalReportRepository
    {
        Task<IEnumerable<MedicalReport>> GetAllAsync();
        Task<MedicalReport> GetByIdAsync(int id);
        Task AddAsync(MedicalReport report);
        Task UpdateAsync(MedicalReport report);
        Task DeleteAsync(int id);
        Task<IEnumerable<MedicalReport>> GetAsync(
            Expression<Func<MedicalReport, bool>> filter,
            Func<IQueryable<MedicalReport>, IOrderedQueryable<MedicalReport>> orderBy,
            string includeProperties = "");
        Task<MedicalReport> GetFirstOrDefaultAsync(
            Expression<Func<MedicalReport, bool>> filter,
            string includeProperties = "");
    }
}