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
        private IUserContext _userContext;

        public ProcessingHouse(IUnitOfWork contextDB, IUserContext userContext) : base(contextDB)
        {
            _userContext = userContext;

            if (_userContext == null)
                throw new ArgumentNullException("userContext");
        }
        public override IDefaultCRUDBLL<Core.Data.Domain.House> GetCRUDHandler()
        {
            return new HouseBLL(contextDB,_userContext);
        }

    }
}
