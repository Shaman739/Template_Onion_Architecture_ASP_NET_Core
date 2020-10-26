using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.DAL;
using System;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.DAL
{
    [TestClass]
    public class UnitOfWorkTest
    {

        [TestMethod]
        public void UpdateItemTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationContextForTest>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;
            ApplicationContextForTest context = new ApplicationContextForTest(options);

            UnitOfWork uow = new UnitOfWork(context);
            ObjectMappingForTest sourceObjectMappingForTest = new ObjectMappingForTest();
            sourceObjectMappingForTest.IntValue = 1;
            sourceObjectMappingForTest.StrValue = "str";
            sourceObjectMappingForTest.SubObject = new SubObjectMappingForTest() { IntValueSub = 2, StrValueSub = "str2" };

            ObjectMappingForTest objectMappingForTest = new ObjectMappingForTest();

            uow.UpdateItem<ObjectMappingForTest>(objectMappingForTest, sourceObjectMappingForTest);

            Assert.AreEqual(sourceObjectMappingForTest.IntValue, objectMappingForTest.IntValue);
            Assert.AreEqual(sourceObjectMappingForTest.StrValue, objectMappingForTest.StrValue);
            Assert.IsNull(objectMappingForTest.SubObject, "Мапятся только простые типы.");

        }
    }
}
