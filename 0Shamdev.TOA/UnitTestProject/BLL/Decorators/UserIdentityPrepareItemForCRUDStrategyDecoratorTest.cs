using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations.Decorators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.BLL.Decorators
{
    [TestClass]
    public class UserIdentityPrepareItemForCRUDStrategyDecoratorTest
    {

        [DataTestMethod,Description("Проверка уставнока значения идентифкатора пользователя")]
        [DataRow(1, null, 1)]
        [DataRow(1, 1000, 1000)]
        public void GetItem_Test(int userIdContext, int? userIdInItem, int resultUserId)
        {
            //ARRANGE
            Mock<IPrepareItemForCRUDStrategy<DomainObjectIdentityForTest>> mockPrepareItem = new Mock<IPrepareItemForCRUDStrategy<DomainObjectIdentityForTest>>();
            mockPrepareItem.Setup(x => x.GetItem(It.IsAny<DefaultParamOfCRUDOperation<DomainObjectIdentityForTest>>())).Returns(new DomainObjectIdentityForTest() {UserId = userIdInItem });

            Mock<IUserContext> mockUserContext = new Mock<IUserContext>();
            mockUserContext.Setup(x => x.GetUserId()).Returns(userIdContext);

            UserIdentityPrepareItemForCRUDStrategyDecorator<DomainObjectIdentityForTest> userIdentityPrepareItemForCRUDStrategyDecorator = new UserIdentityPrepareItemForCRUDStrategyDecorator<DomainObjectIdentityForTest>(mockPrepareItem.Object, mockUserContext.Object);

            DefaultParamOfCRUDOperation<DomainObjectIdentityForTest> item = new DefaultParamOfCRUDOperation<DomainObjectIdentityForTest>() { Item = new DomainObjectIdentityForTest() };
            //ACT
            DomainObjectIdentityForTest result =userIdentityPrepareItemForCRUDStrategyDecorator.GetItem(item);

            //ASSERT

            Assert.AreEqual(resultUserId, result.UserId);

        }
    }
}
