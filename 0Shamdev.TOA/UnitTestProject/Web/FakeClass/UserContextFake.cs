using Shamdev.TOA.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.Web.FakeClass
{
    class UserContextFake : IUserContext
    {
        public long GetUserId()
        {
            return 1;
        }
    }
}
