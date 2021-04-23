using Shamdev.ERP.Core.Data.Interface;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL.PrepareItemForCRUDOperations.Decorators
{
    public class UserIdentityPrepareItemForCRUDStrategyFactoryDecorator<TEntity> : IPrepareItemForCRUDStrategyFactory<TEntity>
        where TEntity : DomainObject, IDomainObjectIdentity , new()
    {
        private IPrepareItemForCRUDStrategyFactory<TEntity> _prepareItemForCRUDStrategyFactory;
        private IUserContext _userContext;

        public UserIdentityPrepareItemForCRUDStrategyFactoryDecorator(IPrepareItemForCRUDStrategyFactory<TEntity> prepareItemForCRUDStrategyFactory, IUserContext userContext)
        {
            _prepareItemForCRUDStrategyFactory = prepareItemForCRUDStrategyFactory;
            _userContext = userContext;

            if (_prepareItemForCRUDStrategyFactory == null)
                throw new ArgumentNullException("prepareItemForCRUDStrategyFactory");

            if (_userContext == null)
                throw new ArgumentNullException("userContext");

            DecorateAllStrategy();
        }

        public HashSet<ExecuteTypeConstCRUD> GetAllStrategiesTypes()
        {
            return _prepareItemForCRUDStrategyFactory.GetAllStrategiesTypes();
        }

        public IPrepareItemForCRUDStrategy<TEntity> GetStrategy(ExecuteTypeConstCRUD executeType)
        {
            return _prepareItemForCRUDStrategyFactory.GetStrategy(executeType);
        }

        public BaseResultType<PrepareItemResult<TEntity>> PrepareItem(DefaultParamOfCRUDOperation<TEntity> queryObject, ExecuteTypeConstCRUD executeType)
        {
            return _prepareItemForCRUDStrategyFactory.PrepareItem(queryObject, executeType);
        }

        public void RemoveStrategy(ExecuteTypeConstCRUD executeType)
        {
            _prepareItemForCRUDStrategyFactory.RemoveStrategy(executeType);
        }

        public void ReplaceStrategy(ExecuteTypeConstCRUD executeType, IPrepareItemForCRUDStrategy<TEntity> newStrategy)
        {
            
            _prepareItemForCRUDStrategyFactory.ReplaceStrategy(executeType, new UserIdentityPrepareItemForCRUDStrategyDecorator<TEntity>(newStrategy, _userContext));
        }

        private void DecorateAllStrategy()
        {
            foreach (var strategy in GetAllStrategiesTypes())
                ReplaceStrategy(strategy, GetStrategy(strategy));
        }
    }
}
