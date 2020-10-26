using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;

namespace Shamdev.TOA.BLL.Validate.Interface
{
    public interface IValidateDomainObject<TEntity>
        where TEntity : DomainObject
    {
        BaseResultType Validate(TEntity item);
    }
}
