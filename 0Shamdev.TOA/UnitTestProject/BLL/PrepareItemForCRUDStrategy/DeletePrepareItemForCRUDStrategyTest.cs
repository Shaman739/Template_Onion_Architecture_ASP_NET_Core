﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations;
using Shamdev.TOA.DAL;
using System;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.BLL.PrepareItemForCRUDStrategy
{
    [TestClass]
    public class DeletePrepareItemForCRUDStrategyTest
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
            Assert.AreEqual(1, uow.Repository<ObjectMappingForTest>().FetchData(null).TotalCountRows);
            //Такого объекта нет в БД. Должна быть ошибка
            ObjectMappingForTest sourceObjectMappingForTest = new ObjectMappingForTest();
            sourceObjectMappingForTest.Id = 2;
            sourceObjectMappingForTest.IntValue = 1;
            sourceObjectMappingForTest.StrValue = "str";

            DeletePrepareItemForCRUDStrategy<ObjectMappingForTest> deletePrepareItemForCRUDStrategy = new DeletePrepareItemForCRUDStrategy<ObjectMappingForTest>(uow);
            ObjectMappingForTest objectMappingForTest;
            Assert.ThrowsException<ArgumentException>(() => objectMappingForTest = deletePrepareItemForCRUDStrategy.GetItem(sourceObjectMappingForTest), "Будет ошибка, так как это изменение и не указан id записи. В БД есть только с id = 1");

            sourceObjectMappingForTest.Id = 1;
            objectMappingForTest = deletePrepareItemForCRUDStrategy.GetItem(sourceObjectMappingForTest);
            uow.SaveChanges();
            Assert.AreEqual(0, uow.Repository<ObjectMappingForTest>().FetchData(null).TotalCountRows);

        }
    }
}
