using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;

namespace Shamdev.TOA.BLL.Infrastructure.ResultType
{
    /// <summary>
    /// Результат CRUD операции
    /// </summary>
    public class SaveResultType<TEntity>
        where TEntity : DomainObject
    {
        /// <summary>
        /// Сохраненный объект
        /// </summary>
        public TEntity Item { get; set; }
    }
}
