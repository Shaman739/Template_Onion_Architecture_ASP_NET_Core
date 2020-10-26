using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.ValidateContext;

namespace Shamdev.TOA.DAL
{
    public interface IApplicationContext
    {
        int SaveChanges();
        void UpdateItem<TEntity>(TEntity toItem, TEntity fromItem) where TEntity : DomainObject;
        ValidateContextResult ValidateItem<TEntity>(TEntity item) where TEntity : DomainObject;
        void Dispose();
    }
}