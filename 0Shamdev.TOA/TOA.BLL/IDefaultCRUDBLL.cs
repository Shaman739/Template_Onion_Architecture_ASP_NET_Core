using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Infrastructure;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL
{
    public interface IDefaultCRUDBLL<TEntity> where TEntity : DomainObject,new()
    {
        Task<ResultFetchData<TEntity>> FetchDataAsync(FetchDataParameters paramQuery);
        Task<BaseResultType<SaveResultType<TEntity>>> SaveItemAsync(ExecuteTypeConstCRUD executeTypeCRUD, DefaultParamOfCRUDOperation<TEntity> paramOfCRUDOperation);

        bool IsOnlyAddInContext { get; set; }

        Task<BaseResultType<TEntity>> GetByIdAsync(long id);
    }
}