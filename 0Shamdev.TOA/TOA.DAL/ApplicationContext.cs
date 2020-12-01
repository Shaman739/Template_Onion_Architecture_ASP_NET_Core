

using Microsoft.EntityFrameworkCore;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.ValidateContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shamdev.TOA.DAL
{
    public class ApplicationContext : DbContext, IApplicationContext
    {
        private Dictionary<Type, Type> _repositoriesType;

        public ApplicationContext(DbContextOptions<ApplicationContext> options) 
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        protected ApplicationContext(DbContextOptions options) 
         : base(options)
        {
            _repositoriesType = new Dictionary<Type, Type>();
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        int IApplicationContext.SaveChanges()
        {
            return base.SaveChanges();
        }

        /// <summary>
        /// Регистрация кастомных типов репозиторий.
        /// </summary>
        /// <typeparam name="TEntity">Тип доменного класса</typeparam>
        /// <typeparam name="TRepository">Репозиторий для этого класса</typeparam>
        protected void RegisterCustomReposynoryType<TEntity, TRepository>()
          where TEntity : DomainObject
          where TRepository : IRepository<TEntity>
        {
            _repositoriesType.Add(typeof(TEntity), typeof(TRepository));
        }
        public Dictionary<Type, Type> GetRepositoriesType()
        {
            return _repositoriesType;
        }
        /// <summary>
        /// Маппинг полей в объекте
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="toItem"></param>
        /// <param name="fromItem"></param>
        public void UpdateItem<TEntity>(TEntity toItem, TEntity fromItem) where TEntity : DomainObject
        {
          //  this.Entry(toItem).CurrentValues.SetValues(fromItem);
            var typefromItem = fromItem.GetType();
            var typetoItem = toItem.GetType();
            var properties = typefromItem.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(info => !info.PropertyType.IsClass ||info.PropertyType == typeof(String) );
            foreach (var pi in properties)
            {
                var selfValue = typefromItem.GetProperty(pi.Name).GetValue(fromItem, null);
                typetoItem.GetProperty(pi.Name).SetValue(toItem, selfValue);
            }
        }

        /// <summary>
        /// Валидация объекта по контексту на обязательные поля
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public ValidateContextResult ValidateItem<TEntity>(TEntity item) where TEntity : DomainObject
        {
            return new ValidateItemInContext<TEntity>(this).Validate(item);
        }
    }
}
