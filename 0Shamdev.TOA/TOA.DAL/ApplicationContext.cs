

using Microsoft.EntityFrameworkCore;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.DAL.ValidateContext;

namespace Shamdev.TOA.DAL
{
    public class ApplicationContext : DbContext, IApplicationContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        protected ApplicationContext(DbContextOptions options)
         : base(options)
        {
        }

        int IApplicationContext.SaveChanges()
        {
            return base.SaveChanges();
        }

        /// <summary>
        /// Маппинг полей в объекте
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="toItem"></param>
        /// <param name="fromItem"></param>
        public void UpdateItem<TEntity>(TEntity toItem, TEntity fromItem) where TEntity : DomainObject
        {
            this.Entry(toItem).CurrentValues.SetValues(fromItem);
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
