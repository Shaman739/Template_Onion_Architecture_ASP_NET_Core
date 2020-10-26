using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.Infrastructure;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL
{
    public interface IDefaultCRUDBLL<TEntity> where TEntity : DomainObject
    {
        Task<ResultFetchData<TEntity>> FetchDataAsync(FetchDataParameters paramQuery);
        Task<SaveResultType<TEntity>> SaveItemAsync(byte executeTypeCRUD, DefaultParamOfCRUDOperation<TEntity> paramOfCRUDOperation);
        Task<PrepareItemResult<TEntity>> GetByIdAsync(long id);
    }
}