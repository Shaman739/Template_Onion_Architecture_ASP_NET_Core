using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Validate.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Interface;

namespace Shamdev.TOA.BLL.Validate
{
    public class ValidateDomainObjectByDBContext<TEntity> : IValidateDomainObject<TEntity>
        where TEntity : DomainObject
    {
        private IUnitOfWork _contextDB;

        private ValidateDomainObjectByDBContext()
        { }
        public ValidateDomainObjectByDBContext(IUnitOfWork contextDB)
        {
            _contextDB = contextDB;
        }
        public BaseResultType Validate(DefaultParamOfCRUDOperation<TEntity> item)
        {
            return _contextDB.IsValidateContext<TEntity>(item.Item);
        }
    }
}
