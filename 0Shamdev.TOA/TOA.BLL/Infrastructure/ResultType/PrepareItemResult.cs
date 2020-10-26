using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;

namespace Shamdev.TOA.BLL.Infrastructure.ResultType
{
    /// <summary>
    /// Подготовка объекта для CRUD
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PrepareItemResult<TEntity> : BaseResultType
        where TEntity : DomainObject
    {
        /// <summary>
        /// Объекта, который будет сохранен. 
        /// </summary>
        public TEntity Item { get; set; }
    }
}
