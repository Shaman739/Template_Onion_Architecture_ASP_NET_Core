using BLL.Common.House.PrepareStrategy;
using BLL.Common.House.Validate;
using Core.Data.Domain;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations.Utils;
using Shamdev.TOA.BLL.Validate.Interface;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Common.House
{
    public class HouseBLL : DefaultCRUDBLL<Core.Data.Domain.House>
    {
        public HouseBLL(IUnitOfWork contextDB, IUserContext userContext) : base(contextDB, userContext)
        {
            //Заменяем стратегию добавления, так как нужно сохранять с flats
            PrepareItemForCRUDStrategyFactory.Value.ReplaceStrategy(ExecuteTypeConstCRUD.ADD, new FlatsSynchonizeDecorator(PrepareItemForCRUDStrategyFactory.Value.GetStrategy(ExecuteTypeConstCRUD.ADD), new FetchDomainData<Flat>(contextDB), new DefaultCRUDBLL<Flat>(contextDB, userContext)));
            PrepareItemForCRUDStrategyFactory.Value.ReplaceStrategy(ExecuteTypeConstCRUD.EDIT, new FlatsSynchonizeDecorator(PrepareItemForCRUDStrategyFactory.Value.GetStrategy(ExecuteTypeConstCRUD.EDIT), new FetchDomainData<Flat>(contextDB), new DefaultCRUDBLL<Flat>(contextDB, userContext)));

            ValidateDomainObject.Value.AddStrategy(ValidateTypeConstCRUD.ADD_OR_EDIT, new Lazy<IValidateDomainObject<Core.Data.Domain.House>>(() => new HouseValidateDomainObject()));
        }
    }
}
