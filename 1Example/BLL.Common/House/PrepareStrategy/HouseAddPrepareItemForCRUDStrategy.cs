using Core.Data.Domain;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations.Utils;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Common.House.PrepareStrategy
{
    public class HouseAddPrepareItemForCRUDStrategy : AddPrepareItemForCRUDStrategy<Core.Data.Domain.House>
    {
        IDefaultCRUDBLL<Flat> flatBLL;
        IFetchData<Flat> fetchFlatBLL;
        public HouseAddPrepareItemForCRUDStrategy(IUnitOfWork uow) : base(uow)
        {
            flatBLL = new DefaultCRUDBLL<Flat>(uow);
            fetchFlatBLL = new FetchDomainData<Flat>(uow);
        }

        protected override Core.Data.Domain.House CreateItem(Core.Data.Domain.House item)
        {
            Core.Data.Domain.House house = base.CreateItem(item);
            house.Flats = new List<Flat>();
            SynchronizeChildrenObject<Flat> synchronizeChildrenObject = new SynchronizeChildrenObject<Flat>(house.Flats, flatBLL, fetchFlatBLL);
            synchronizeChildrenObject.Synchonize(item.Flats);

            return house;
        }
    }
}
