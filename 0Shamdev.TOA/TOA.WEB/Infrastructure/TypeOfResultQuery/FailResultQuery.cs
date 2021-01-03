using Shamdev.TOA.Core.Data.Infrastructure.ResultType;

namespace Shamdev.TOA.Web.Infrastructure.TypeOfResultQuery
{
    /// <summary>
    /// Неуспешний результат запроса. IsSuccess всегда  = false
    /// </summary>
    public class FailResultQuery : BaseResultType
    {
        public FailResultQuery() : base()
        {
            Status = ResultStatus.Fail;
        }
    }
}
