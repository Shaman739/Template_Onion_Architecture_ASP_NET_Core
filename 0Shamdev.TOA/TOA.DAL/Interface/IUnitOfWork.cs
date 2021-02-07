using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.ValidateContext;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Shamdev.TOA.DAL.Interface
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        IRepository<TEntity> Repository<TEntity>() where TEntity : DomainObject, new();

        ValidateContextResult IsValidateContext<TEntity>(TEntity item) where TEntity : DomainObject;

        void UpdateItem<TEntity>(TEntity toItem, TEntity fromItem) where TEntity : DomainObject;

    }
}