using Core.Data.Domain;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations.Utils;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Common.House.PrepareStrategy
{
    public class HouseEditPrepareItemForCRUDStrategy : EditPrepareItemForCRUDStrategy<Core.Data.Domain.House>
    {
        IDefaultCRUDBLL<Flat> flatBLL;
        public HouseEditPrepareItemForCRUDStrategy(IUnitOfWork uow) : base(uow)
        {
            flatBLL = new DefaultCRUDBLL<Flat>(uow);
        }

        protected override Core.Data.Domain.House CreateItem(Core.Data.Domain.House item)
        {
            Core.Data.Domain.House house = base.CreateItem(item);
            SynchronizeChildrenObject<Flat> synchronizeChildrenObject = new SynchronizeChildrenObject<Flat>(house.Flats, flatBLL);
            synchronizeChildrenObject.Synchonize(item.Flats);

            return house;
        }
    }
}
