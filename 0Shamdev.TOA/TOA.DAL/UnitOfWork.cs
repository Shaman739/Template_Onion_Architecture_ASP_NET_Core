
using Shamdev.TOA.DAL.Interface;

using System;
using System.Collections.Generic;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.ValidateContext;
using System.Linq;
using System.Threading.Tasks;

namespace Shamdev.TOA.DAL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IApplicationContext _contextDB;
      
        private UnitOfWork()
        {
           
        }
        public UnitOfWork(IApplicationContext contextDB) : this()
        {
            _contextDB = contextDB;
        }
        private Dictionary<string, dynamic> _repositoriesCreated;

      
        /// <summary>
        /// Получения репозитория для CRUD операций или выборки данных
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IRepository<TEntity> Repository<TEntity>()
            where TEntity : DomainObject,new()
        {
            if (_repositoriesCreated == null)
                _repositoriesCreated = new Dictionary<string, dynamic>();
            var type = typeof(TEntity).Name;
            if (_repositoriesCreated.ContainsKey(type))
                return (IRepository<TEntity>)_repositoriesCreated[type];
            Type repositoryType = _contextDB.GetRepositoriesType().FirstOrDefault(x => x.Key == typeof(TEntity)).Value;

            if (repositoryType != null)
            {
                _repositoriesCreated.Add(type, Activator.CreateInstance(repositoryType, _contextDB));
            }
            else
            {
                repositoryType = typeof(Repository<>);
                _repositoriesCreated.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _contextDB));
                
            }          
            
            return (IRepository<TEntity>)_repositoriesCreated[type];
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _contextDB.SaveChangesAsync();
        }

        public void Dispose()
        {
            _repositoriesCreated?.Clear();
            _contextDB.Dispose();

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
