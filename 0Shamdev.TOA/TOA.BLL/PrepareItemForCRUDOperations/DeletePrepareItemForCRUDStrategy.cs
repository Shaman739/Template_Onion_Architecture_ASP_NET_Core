using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.Interface;

namespace Shamdev.TOA.BLL.PrepareItemForCRUDOperations
{
    /// <summary>
    /// Подготовка объекта для удаления из БД. 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DeletePrepareItemForCRUDStrategy<TEntity> : AbstarctPrepareItemForCRUDStrategy<TEntity>
        where TEntity : DomainObject, new()
    {
        public DeletePrepareItemForCRUDStrategy(IUnitOfWork uow) : base(uow)
        {

        }
        public sealed override TEntity GetItem(DefaultParamOfCRUDOperation<TEntity> item)
        {
            TEntity itemNew = CreateItem(item.Item);

            return itemNew;
        }

        protected virtual TEntity CreateItem(TEntity item)
        {
            return dao.Delete(item.Id);
        }
    }
}
