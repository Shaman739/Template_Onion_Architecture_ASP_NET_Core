using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.WEB.Cache.Interface
{
    public interface ICache<TEntity>
        where TEntity:DomainObject,new()
    {
        Task<bool> AddAsync(TEntity item);
        Task<bool> UpdateAsync(TEntity item);
        Task<bool> DeleteAsync(TEntity item);
        Task<BaseResultType<TEntity>> GetByIdAsync(long id);

    }
}
