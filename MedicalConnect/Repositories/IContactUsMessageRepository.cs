using MedicalConnect.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MedicalConnect.Repositories
{
    public interface IContactUsMessageRepository : IRepository<ContactUsMessage>
    {
        Task<IEnumerable<ContactUsMessage>> GetRecentMessagesAsync(int count);
    }
}