using MedicalConnect.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public class ContactUsMessageRepository : Repository<ContactUsMessage>, IContactUsMessageRepository
    {
        private readonly ApplicationDbContext _db;

        public ContactUsMessageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ContactUsMessage>> GetRecentMessagesAsync(int count)
        {
            return await _db.ContactUsMessages
                .OrderByDescending(m => m.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}