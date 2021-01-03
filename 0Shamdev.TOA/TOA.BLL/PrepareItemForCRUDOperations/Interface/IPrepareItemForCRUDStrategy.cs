using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;

namespace Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface
{
    public interface IPrepareItemForCRUDStrategy<TEntity> where TEntity : DomainObject
    {
        TEntity GetItem(DefaultParamOfCRUDOperation<TEntity> item);
    }
}
