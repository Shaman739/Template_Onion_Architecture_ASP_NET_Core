using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Common.House
{
    public class ProcessingHouse : ProcessingDomainObject<Core.Data.Domain.House>
    {
        public ProcessingHouse(IUnitOfWork contextDB) : base(contextDB)
        {

        }
        public override IDefaultCRUDBLL<Core.Data.Domain.House> GetCRUDHandler()
        {
            return new HouseBLL(contextDB);
        }

    }
}
