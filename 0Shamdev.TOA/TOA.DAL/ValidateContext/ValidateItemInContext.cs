using Microsoft.EntityFrameworkCore;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Attribute;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System;
using System.Linq;
using System.Reflection;

namespace Shamdev.TOA.DAL.ValidateContext
{
    /// <summary>
    /// Валидация объекта через контекст по обязательным полям, который помечены через атрибут [Requared] или  modelBuilder.Entity<DomainObject>().Property(b => b.Number).IsRequired() в protected override void OnModelCreating(ModelBuilder modelBuilder)
    /// </summary>
    /// <typeparam name="TEntity">Тип доменного класса</typeparam> 
    public class ValidateItemInContext<TEntity>
        where TEntity : DomainObject
    {
        DbContext _applicationContext;
        public ValidateItemInContext(DbContext applicationContext)
        {
            _applicationContext = applicationContext;
        }
        /// <summary>
        /// Порядок проверки. 
        /// 1 - проверка наличия объекта в контексте.
        /// 2 - проверка по notNull полям
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ValidateContextResult Validate(TEntity item)
        {

            ValidateContextResult result = new ValidateContextResult();
            result.Status = ResultStatus.Success;

            if (_applicationContext.Set<TEntity>().Local.FirstOrDefault(x => x == item) == null)
            {
                result.AddError("Объект не найден в контексте для проверки обязательных полей");
            }
            else
            {
                var entry = _applicationContext.Entry(item);
                var properties = _applicationContext.Entry(item).Metadata.GetProperties();
                ValidateContextResultItem validateContextResultItem = new ValidateContextResultItem()
                {
                    Name = GetNameFieldOrClass(entry.Entity.GetType())
                };

                foreach (var property in properties)
                {
                    //TODO: Сделать валидацию вложенных объектов
                    // if (entry.Entity is DomainObject)
                    //      result.Merge(Validate(entry.Entity));
                    // else
                    //  {
                    var value = property.PropertyInfo?.GetValue(entry.Entity);
                    if (!property.IsNullable && value == null)
                    {
                        validateContextResultItem.Fields.Add(new ValidateContextResultItem()
                        {
                            Name = GetNameFieldOrClass(property.PropertyInfo)
                        });

                    }
                    //  }
                }
                if (validateContextResultItem.Fields.Count() > 0)
                {
                    result.AddEntity(validateContextResultItem);
                }
            }

            return result;

        }

        /// <summary>
        /// Формирование имени поля или объекта по атрибуту [Summary], чтобы вывести сообщение понятное для пользователя.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected string GetNameFieldOrClass(MemberInfo field)
        {
            if (field == null)
                return String.Empty;

            var attributes = field.GetCustomAttributes(typeof(SummaryAttribute), true).ToList();
            string nameField;
            if (attributes.Count > 0)
                nameField = ((SummaryAttribute)attributes[0]).Summary;
            else
                nameField = field.Name; ;

            return nameField;
        }
    }
}
