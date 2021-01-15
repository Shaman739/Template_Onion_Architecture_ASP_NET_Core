using Microsoft.EntityFrameworkCore;
using Shamdev.ERP.Core.Data.Domain;
using Shamdev.ERP.DAL.Common.Repository.Interface;
using Shamdev.TOA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.ERP.DAL.Common.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DbContext contextDB) : base(contextDB)
        {
        }

        public Task<User> CheckIssueUserAsync(string email, string password)
        {
            return DbSetWithInclude.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        }
    }
}
