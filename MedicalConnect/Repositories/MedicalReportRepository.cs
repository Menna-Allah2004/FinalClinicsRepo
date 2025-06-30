using MedicalConnect.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public class MedicalReportRepository : IMedicalReportRepository
    {
        private readonly ApplicationDbContext _db;

        public MedicalReportRepository(ApplicationDbContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<MedicalReport>> GetAsync(
    Expression<Func<MedicalReport, bool>> filter,
    Func<IQueryable<MedicalReport>, IOrderedQueryable<MedicalReport>> orderBy,
    string includeProperties = "")
        {
            IQueryable<MedicalReport> query = _db.MedicalReports;

            // Apply the filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Include related properties if provided
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            // Apply the ordering if provided
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Return the results
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<MedicalReport>> GetAllAsync()
        {
            return await _db.MedicalReports.ToListAsync();
        }

        public async Task<MedicalReport> GetFirstOrDefaultAsync(
        Expression<Func<MedicalReport, bool>> filter,
        string includeProperties = "")
        {
            IQueryable<MedicalReport> query = _db.MedicalReports;

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<MedicalReport> GetByIdAsync(int id)
        {
            return await _db.MedicalReports.FindAsync(id);
        }

        public async Task AddAsync(MedicalReport report)
        {
            await _db.MedicalReports.AddAsync(report);
        }

        public async Task UpdateAsync(MedicalReport report)
        {
            _db.MedicalReports.Update(report);
        }

        public async Task DeleteAsync(int id)
        {
            var report = await _db.MedicalReports.FindAsync(id);
            if (report != null)
            {
                _db.MedicalReports.Remove(report);
            }
        }
    }
}