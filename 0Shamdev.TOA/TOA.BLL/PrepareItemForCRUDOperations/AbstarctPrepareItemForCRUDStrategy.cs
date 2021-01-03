using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Interface;

namespace Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations
{
    public abstract class AbstarctPrepareItemForCRUDStrategy<TEntity> : IPrepareItemForCRUDStrategy<TEntity>
        where TEntity : DomainObject, new()
    {
        protected IRepository<TEntity> dao;
        protected IUnitOfWork uow;
        public AbstarctPrepareItemForCRUDStrategy(IUnitOfWork uow)
        {
            this.uow = uow;
            this.dao = uow.Repository<TEntity>();
        }

        public abstract TEntity GetItem(DefaultParamOfCRUDOperation<TEntity> item);



    }
}
