using Shamdev.ERP.Core.Data.Interface;
using Shamdev.TOA.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.DAL.TestFakeClasses
{
    public class DomainObjectIdentityForTest : DomainObject,IDomainObjectIdentity
    {
        public long? UserId { get; set; }
    }
}
