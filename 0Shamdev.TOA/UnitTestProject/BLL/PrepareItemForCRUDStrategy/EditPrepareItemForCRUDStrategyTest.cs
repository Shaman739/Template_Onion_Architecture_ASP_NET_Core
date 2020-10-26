using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations;
using Shamdev.TOA.DAL;
using System;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.BLL.PrepareItemForCRUDStrategy
{
    [TestClass]
    public class EditPrepareItemForCRUDStrategyTest
    {
        [TestMethod]
        public void GetItemTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationContextForTest>()
                          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                         .Options;
            ApplicationContextForTest context = new ApplicationContextForTest(options);
            context.Add(new ObjectMappingForTest() { Id = 1, IntValue = 2, StrValue = "2", SubObject = new SubObjectMappingForTest() { Id = 33, IntValueSub = 34, StrValueSub = "35" } });
            UnitOfWork uow = new UnitOfWork(context);
            uow.SaveChanges();
            ObjectMappingForTest sourceObjectMappingForTest = new ObjectMappingForTest();
            sourceObjectMappingForTest.IntValue = 1;
            sourceObjectMappingForTest.StrValue = "str";
            //Это не должно мапится
            sourceObjectMappingForTest.SubObject = new SubObjectMappingForTest() { IntValueSub = 2, StrValueSub = "str2" };

            EditPrepareItemForCRUDStrategy<ObjectMappingForTest> addPrepareItemForCRUDStrategy = new EditPrepareItemForCRUDStrategy<ObjectMappingForTest>(uow);
            ObjectMappingForTest objectMappingForTest;
            Assert.ThrowsException<Exception>(() => objectMappingForTest = addPrepareItemForCRUDStrategy.GetItem(sourceObjectMappingForTest), "Будет ошибка, так как это изменение и не указан id записи. В БД есть только с id = 1");

            sourceObjectMappingForTest.Id = 1;
            objectMappingForTest = addPrepareItemForCRUDStrategy.GetItem(sourceObjectMappingForTest);
            Assert.AreEqual(sourceObjectMappingForTest.IntValue, objectMappingForTest.IntValue);
            Assert.AreEqual(sourceObjectMappingForTest.StrValue, objectMappingForTest.StrValue);
            Assert.IsNotNull(objectMappingForTest.SubObject, "Мапятся только простые типы.");
            //Мапятся только простые типы. Данные во вложенных объектах не мапятся
            Assert.AreEqual(33, objectMappingForTest.SubObject.Id);
            Assert.AreEqual(34, objectMappingForTest.SubObject.IntValueSub);
            Assert.AreEqual("35", objectMappingForTest.SubObject.StrValueSub);

        }
    }
}
