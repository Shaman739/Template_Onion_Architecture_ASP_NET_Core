using Core.Data.Domain;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Common.House.PrepareStrategy
{

    class FlatsSynchonizeDecorator : IPrepareItemForCRUDStrategy<Core.Data.Domain.House>
    {
        IPrepareItemForCRUDStrategy<Core.Data.Domain.House> _prepareItemForCRUDStrategy;
        IDefaultCRUDBLL<Flat> _flatBLL;
        IFetchData<Flat> _fetchFlatBLL;
        public FlatsSynchonizeDecorator(IPrepareItemForCRUDStrategy<Core.Data.Domain.House> prepareItemForCRUDStrategy, IFetchData<Flat> fetchData, IDefaultCRUDBLL<Flat> flatBll)
        {
            _prepareItemForCRUDStrategy = prepareItemForCRUDStrategy;
            _fetchFlatBLL = fetchData;
            _flatBLL = flatBll;
            if (_prepareItemForCRUDStrategy == null)
                throw new ArgumentNullException("prepareItemForCRUDStrategy");
            if (_flatBLL == null)
                throw new ArgumentNullException("flatBLL");
            if (_fetchFlatBLL == null)
                throw new ArgumentNullException("fetchFlatBLL");

        }
        public Core.Data.Domain.House GetItem(DefaultParamOfCRUDOperation<Core.Data.Domain.House> item)
        {
            Core.Data.Domain.House house = _prepareItemForCRUDStrategy.GetItem(item);

            if (house != null)
            {
                if (house.Flats == null)
                    house.Flats = new List<Flat>();

                SynchronizeChildrenObject<Flat> synchronizeChildrenObject = new SynchronizeChildrenObject<Flat>(house.Flats, _flatBLL, _fetchFlatBLL);
                synchronizeChildrenObject.Synchonize(item.Item.Flats);
            }
            return house;
        }
    }
}
