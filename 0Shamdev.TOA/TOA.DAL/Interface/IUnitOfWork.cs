using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.ValidateContext;

namespace Shamdev.TOA.DAL.Interface
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        IRepository<TEntity> Repository<TEntity>() where TEntity : DomainObject;

        ValidateContextResult IsValidateContext<TEntity>(TEntity item) where TEntity : DomainObject;


        void UpdateItem<TEntity>(TEntity toItem, TEntity fromItem) where TEntity : DomainObject;
    }
}