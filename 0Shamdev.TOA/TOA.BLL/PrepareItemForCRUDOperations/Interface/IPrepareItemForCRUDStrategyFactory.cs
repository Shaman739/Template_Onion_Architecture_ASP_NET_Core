using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;

namespace Shamdev.TOA.BLL.PrepareItemForCRUDOperations.Interface
{
    public interface IPrepareItemForCRUDStrategyFactory<TEntity> where TEntity : DomainObject, new()
    {
        IPrepareItemForCRUDStrategy<TEntity> GetStrategy(ExecuteTypeConstCRUD executeType);
        BaseResultType<PrepareItemResult<TEntity>> PrepareItem(DefaultParamOfCRUDOperation<TEntity> queryObject, ExecuteTypeConstCRUD executeType);
        void RemoveStrategy(ExecuteTypeConstCRUD executeType);
        void ReplaceStrategy(ExecuteTypeConstCRUD executeType, IPrepareItemForCRUDStrategy<TEntity> newStrategy);
    }
}