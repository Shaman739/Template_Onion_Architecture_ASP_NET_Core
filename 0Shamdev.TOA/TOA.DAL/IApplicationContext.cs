using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.ValidateContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shamdev.TOA.DAL
{
    public interface IApplicationContext
    {
        Task<int> SaveChangesAsync();
        void UpdateItem<TEntity>(TEntity toItem, TEntity fromItem) where TEntity : DomainObject;
        ValidateContextResult ValidateItem<TEntity>(TEntity item) where TEntity : DomainObject;
        void Dispose();
        Dictionary<Type, Type> GetRepositoriesType();
    }
}