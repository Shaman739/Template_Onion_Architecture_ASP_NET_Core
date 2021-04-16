using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shamdev.ERP.DAL.Common.Interface;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Decorators;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.BLL.Decorators
{
    [TestClass]
    public class UserDataSecurityFetchDomainDataTest
    {
        [TestMethod]
        public void FetchDataAsyncTest_AddParamUserId()
        {

            //ARRANGE

            FetchDataParameters param = new FetchDataParameters();

            Mock<IFetchData<DomainObjectIdentityForTest>> mockFetchData = new Mock<IFetchData<DomainObjectIdentityForTest>>();
            mockFetchData.Setup(x => x.FetchDataAsync(param));

            long userIdContext = 2;
            Mock<IUserContext> mockUserContext = new Mock<IUserContext>();
            mockUserContext.Setup(x => x.GetUserId()).Returns(userIdContext);

            UserDataSecurityFetchDomainData<DomainObjectIdentityForTest> userDataSecurityFetchDomainData = new UserDataSecurityFetchDomainData<DomainObjectIdentityForTest>(mockFetchData.Object, mockUserContext.Object);

            //ACT
            userDataSecurityFetchDomainData.FetchDataAsync(param);

            //ASSERT

            Assert.AreEqual(1, param.Filters.Count);
            Assert.AreEqual("2", param.Filters.First().Value);
            Assert.AreEqual("UserId", param.Filters.First().PropertyName);
            Assert.AreEqual(TypeFilterEnum.EQUAL, param.Filters.First().TypeFilter);

        }
    }
}
