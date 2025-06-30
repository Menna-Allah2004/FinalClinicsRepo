using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null);
        Task<T> GetByIdAsync(int id);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null,
            string includeProperties = null);
        Task AddAsync(T entity);
        Task RemoveAsync(int id);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
    }
}