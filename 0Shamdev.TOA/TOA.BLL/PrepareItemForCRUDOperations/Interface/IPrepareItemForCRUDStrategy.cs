using Shamdev.TOA.Core.Data;

namespace Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface
{
    public interface IPrepareItemForCRUDStrategy<TEntity> where TEntity : DomainObject
    {
        TEntity GetItem(TEntity item);
    }
}
