using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.Interface;

namespace Shamdev.TOA.BLL.PrepareItemForCRUDOperations
{
    /// <summary>
    /// Подготовка объекта для сохранения в БД. 
    /// Порядок действий:
    /// 1 - создает обхект и добавляет в контекст
    /// 2 - мапит данные в новый объект из объекта, который пришел с клиента
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class AddPrepareItemForCRUDStrategy<TEntity> : AbstarctPrepareItemForCRUDStrategy<TEntity>
        where TEntity : DomainObject, new()
    {
        public AddPrepareItemForCRUDStrategy(IUnitOfWork uow) : base(uow)
        {

        }
        public sealed override TEntity GetItem(DefaultParamOfCRUDOperation<TEntity> item)
        {
            TEntity itemNew = CreateItem(item.Item);
            item.Item.Id = itemNew.Id;
            uow.UpdateItem<TEntity>(itemNew, item.Item);
            return itemNew;
        }

        protected virtual TEntity CreateItem(TEntity item)
        {
            return dao.Create();
        }
    }
}
