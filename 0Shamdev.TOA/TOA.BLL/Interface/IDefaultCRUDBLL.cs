using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Infrastructure;
using System.Threading.Tasks;
using Shamdev.TOA.BLL;

namespace Shamdev.TOA.BLL.Interface
{
    public interface IDefaultCRUDBLL<TEntity> where TEntity : DomainObject,new()
    {
        Task<BaseResultType<SaveResultType<TEntity>>> SaveItemAsync(ExecuteTypeConstCRUD executeTypeCRUD, DefaultParamOfCRUDOperation<TEntity> paramOfCRUDOperation);
        bool IsOnlyAddInContext { get; set; }

        delegate void DomainChangeHandler<T>(ExecuteTypeConstCRUD executeTypeCRUD, T item) where T : DomainObject;
       
        event DomainChangeHandler<TEntity> DomainChangeEvent;
    }
}