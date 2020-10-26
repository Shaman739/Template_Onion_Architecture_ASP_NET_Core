
using Shamdev.TOA.DAL.Interface;

using System;
using System.Collections.Generic;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.ValidateContext;

namespace Shamdev.TOA.DAL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IApplicationContext _contextDB;
        public UnitOfWork(IApplicationContext contextDB)
        {
            _contextDB = contextDB;
        }

        private Dictionary<string, dynamic> _repositories;
        /// <summary>
        /// Получения репозитория для CRUD операций или выборки данных
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IRepository<TEntity> Repository<TEntity>()
            where TEntity : DomainObject
        {
            if (_repositories == null)
                _repositories = new Dictionary<string, dynamic>();
            var type = typeof(TEntity).Name;
            if (_repositories.ContainsKey(type))
                return (IRepository<TEntity>)_repositories[type];
            var repositoryType = typeof(Repository<>);
            _repositories.Add(type, Activator.CreateInstance(
                repositoryType.MakeGenericType(typeof(TEntity)), _contextDB)
            );
            return (IRepository<TEntity>)_repositories[type];
        }

        public int SaveChanges()
        {
            return _contextDB.SaveChanges();
        }

        public void Dispose()
        {
            _repositories?.Clear();
            _contextDB.Dispose();
            GC.Collect();

        }
        /// <summary>
        ///  Валидация объекта по контексту на обязательные поля
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public ValidateContextResult IsValidateContext<TEntity>(TEntity item) where TEntity : DomainObject
        {
            return _contextDB.ValidateItem<TEntity>(item);
        }

        /// <summary>
        /// Мапинг значений с одного объета в другой
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="toItem">Заполняемый объект</param>
        /// <param name="fromItem">Объект-источник новый данных</param>
        public void UpdateItem<TEntity>(TEntity toItem, TEntity fromItem) where TEntity : DomainObject
        {
            _contextDB.UpdateItem<TEntity>(toItem, fromItem);
        }
    }
}
