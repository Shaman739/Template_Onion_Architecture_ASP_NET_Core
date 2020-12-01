using BLL.Common.House.PrepareStrategy;
using Core.Data.Domain;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Common.House
{
    public class HouseBLL : DefaultCRUDBLL<Core.Data.Domain.House>
    {
        public HouseBLL(IUnitOfWork contextDB) : base(contextDB)
        {
            //Заменяем стратегию добавления, так как нужно сохранять с flats
            PrepareItemForCRUDStrategyFactory.Value.ReplaceStrategy(ExecuteTypeConstCRUD.ADD,new HouseAddPrepareItemForCRUDStrategy(contextDB));
            PrepareItemForCRUDStrategyFactory.Value.ReplaceStrategy(ExecuteTypeConstCRUD.EDIT, new HouseEditPrepareItemForCRUDStrategy(contextDB));
        }
    }
}
