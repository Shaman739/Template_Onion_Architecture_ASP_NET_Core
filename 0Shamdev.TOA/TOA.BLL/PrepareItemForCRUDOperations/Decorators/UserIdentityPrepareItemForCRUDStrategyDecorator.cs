using Shamdev.ERP.Core.Data.Interface;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.Core.Data;
using System;

namespace Shamdev.TOA.BLL.PrepareItemForCRUDOperations.Decorators
{
    public class UserIdentityPrepareItemForCRUDStrategyDecorator<TEntity> : IPrepareItemForCRUDStrategy<TEntity>
        where TEntity : DomainObject, IDomainObjectIdentity, new()
    {
        IPrepareItemForCRUDStrategy<TEntity> _prepareItemForCRUDStrategy;
        private IUserContext _userContext;

        public UserIdentityPrepareItemForCRUDStrategyDecorator(IPrepareItemForCRUDStrategy<TEntity> prepareItemForCRUDStrategy, IUserContext userContext)
        {
            this._prepareItemForCRUDStrategy = prepareItemForCRUDStrategy;
            _userContext = userContext;

            if (_prepareItemForCRUDStrategy == null)
                throw new ArgumentNullException("prepareItemForCRUDStrategy");

            if (_userContext == null)
                throw new ArgumentNullException("userContext");
        }

        public TEntity GetItem(DefaultParamOfCRUDOperation<TEntity> item)
        {
            TEntity itemNew = _prepareItemForCRUDStrategy.GetItem(item);

            if(itemNew.UserId is null)
                itemNew.UserId = _userContext.GetUserId();

            return itemNew;

        }
    }
}