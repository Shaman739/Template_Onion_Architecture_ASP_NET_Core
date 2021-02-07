using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations;
using Shamdev.TOA.DAL;
using System;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.BLL.PrepareItemForCRUDStrategy
{
    [TestClass]
    public class AddPrepareItemForCRUDStrategyTest
    {
        [TestMethod]
        public void GetItemTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationContextForTest>()
                          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                         .Options;
            ApplicationContextForTest context = new ApplicationContextForTest(options);

            UnitOfWork uow = new UnitOfWork(context);
            uow.SaveChangesAsync();
            DefaultParamOfCRUDOperation<ObjectMappingForTest> sourceObjectMappingForTest = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            sourceObjectMappingForTest.Item = new ObjectMappingForTest();
            sourceObjectMappingForTest.Item.IntValue = 1;
            sourceObjectMappingForTest.Item.StrValue = "str";
            sourceObjectMappingForTest.Item.SubObject = new SubObjectMappingForTest() { IntValueSub = 2, StrValueSub = "str2" };

            AddPrepareItemForCRUDStrategy<ObjectMappingForTest> addPrepareItemForCRUDStrategy = new AddPrepareItemForCRUDStrategy<ObjectMappingForTest>(uow);
            ObjectMappingForTest objectMappingForTest = addPrepareItemForCRUDStrategy.GetItem(sourceObjectMappingForTest);

            Assert.AreEqual(sourceObjectMappingForTest.Item.IntValue, objectMappingForTest.IntValue);
            Assert.AreEqual(sourceObjectMappingForTest.Item.StrValue, objectMappingForTest.StrValue);
            Assert.IsNull(objectMappingForTest.SubObject, "Мапятся только простые типы.");
        }
    }
}
