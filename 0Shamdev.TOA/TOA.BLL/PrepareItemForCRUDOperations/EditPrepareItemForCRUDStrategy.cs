using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.Interface;
using System;

namespace Shamdev.TOA.BLL.PrepareItemForCRUDOperations
{
    /// <summary>
    /// Подготовка объекта для изменения в БД. 
    /// Порядок действий:
    /// 1 - получает объект из БД по id, который указан в item
    /// 2 - мапит данные в объект из БД 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EditPrepareItemForCRUDStrategy<TEntity> : AbstarctPrepareItemForCRUDStrategy<TEntity>
        where TEntity : DomainObject, new()
    {
        public EditPrepareItemForCRUDStrategy(IUnitOfWork uow) : base(uow)
        {

        }
        public sealed override TEntity GetItem(TEntity item)
        {
            TEntity itemNew = CreateItem(item);
            if (itemNew == null) throw new Exception("Объект не найден в БД для изменения.");
            uow.UpdateItem<TEntity>(itemNew, item);

            return itemNew;
        }

        protected virtual TEntity CreateItem(TEntity item)
        {
            return dao.GetById(item.Id);
        }
    }
}
