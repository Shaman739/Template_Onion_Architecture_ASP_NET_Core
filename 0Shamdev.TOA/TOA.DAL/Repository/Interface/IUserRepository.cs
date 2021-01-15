using Shamdev.ERP.Core.Data.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.ERP.DAL.Common.Repository.Interface
{
    public interface IUserRepository
    {
        Task<User> CheckIssueUserAsync(string email, string password);
    }
}
