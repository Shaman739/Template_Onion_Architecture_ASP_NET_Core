using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Infrastructure;
using Shamdev.TOA.DAL.Infrastructure.Interface;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL.Interface
{
    public interface IFetchData<TEntity>
    where TEntity : DomainObject, new()

    {
        Task<ResultFetchData<TEntity>> FetchDataAsync(IFetchDataParameters paramQuery);
        Task<BaseResultType<TEntity>> GetByIdAsync(long id);
    }
}