using Shamdev.TOA.Core.Data;

namespace Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD
{
    /// <summary>
    /// Параметры запроса для CRUD
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DefaultParamOfCRUDOperation<TEntity>
        where TEntity : DomainObject
    {
        /// <summary>
        /// Объект для CRUD
        /// </summary>
        public TEntity Item { get; set; }
    }
}
