using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System.Collections.Generic;

namespace Shamdev.TOA.Web.Infrastructure.TypeOfResultQuery
{
    /// <summary>
    /// Результат запроса данных. IsSuccess всегда  = true
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class FetchDataResultQuery<TEntity> : BaseResultType
        where TEntity : DomainObject
    {
        public FetchDataResultQuery() : base()
        {
            Status = ResultStatus.Success;
        }
        /// <summary>
        /// Список объектов на странице
        /// </summary>
        public List<TEntity> Items { get; set; }
        /// <summary>
        /// Всего строк по запросу
        /// </summary>
        public int Count { get; set; }
    }
}
