using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;

namespace Shamdev.TOA.Web.Infrastructure.TypeOfResultQuery
{
    /// <summary>
    /// Результат запроса одной записи. IsSuccess всегда  = true
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ItemResultQuery<TEntity> : BaseResultType
        where TEntity : DomainObject
    {
        public ItemResultQuery() : base()
        {
            Status = ResultStatus.Success;
        }
        public TEntity Item { get; set; }
    }
}
