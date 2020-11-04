﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Infrastructure;
using System;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.DAL
{
    [TestClass]
    public class RepositoryTest
    {
        static Repository<ObjectMappingForTest> repository;
        static ApplicationContextForTest context;
        [TestInitialize]
        public void Init()
        {
            if (repository == null)
            {
                ReCreateContext();
            }
        }
        private void ReCreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContextForTest>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            context = new ApplicationContextForTest(options);

            repository = new Repository<ObjectMappingForTest>(context);
            repository.Add(new ObjectMappingForTest() { Id = 2, IntValue = 2, StrValue = "2" });
            repository.Add(new ObjectMappingForTest() { Id = 3, IntValue = 3, StrValue = "3" });
            repository.Add(new ObjectMappingForTest() { Id = 4, IntValue = 4, StrValue = "4" });
            repository.Add(new ObjectMappingForTest() { Id = 5, IntValue = 5, StrValue = "5" });
            repository.Add(new ObjectMappingForTest() { Id = 6, IntValue = 6, StrValue = "6" });
            repository.Add(new ObjectMappingForTest() { Id = 7, IntValue = 7, StrValue = "7" });

            context.SaveChanges();
        }
        [TestMethod]
        public void GetByIdTest()
        {
            ReCreateContext();
            ObjectMappingForTest item = repository.GetById(2);
            Assert.IsNotNull(item, "Не найден объект по ID.");
            //Провека, что найден именно тот, который с id = 2
            Assert.AreEqual(2, item.Id);
            Assert.AreEqual(2, item.IntValue);
            Assert.AreEqual("2", item.StrValue);

            Assert.IsNull(repository.GetById(20), "Такой записи нет и должно быть исключение.");

        }

        [TestMethod]
        public void CreateTest()
        {
            ObjectMappingForTest item = repository.Create();
            Assert.IsNotNull(item, "Не добавлен объект в контекст.");
            //Провека, что создан пустой объект с дефолтными значениями и следующим id
            Assert.AreEqual(8, item.Id);
            Assert.IsNull(item.IntValue);
            Assert.IsNull(item.StrValue);
            Assert.IsNull(item.SubObject);
        }

        [TestMethod]
        public void FetchDataTest()
        {
            ReCreateContext();
            ResultFetchData<ObjectMappingForTest> resultFetchData = repository.FetchData(null);

            Assert.IsNotNull(resultFetchData, "Результат запроса не должен быть null.");
            //Провека, что создан пустой объект с дефолтными значениями
            Assert.AreEqual(6, resultFetchData.TotalCountRows);
            Assert.AreEqual(6, resultFetchData.Items.Count);

            //Проверяем уже с заданым пейджингом
            resultFetchData = repository.FetchData(new FetchDataParameters(1, 2));

            Assert.IsNotNull(resultFetchData, "Результат запроса не должен быть null.");
            //Проверку, что вернулась первая страница
            Assert.AreEqual(6, resultFetchData.TotalCountRows);
            Assert.AreEqual(2, resultFetchData.Items.Count);
            Assert.AreEqual(2, resultFetchData.Items[0].Id);
            Assert.AreEqual(3, resultFetchData.Items[1].Id);

            //Проверка 2 страницы
            resultFetchData = repository.FetchData(new FetchDataParameters(2, 2));
            Assert.AreEqual(4, resultFetchData.Items[0].Id);
            Assert.AreEqual(5, resultFetchData.Items[1].Id);
        }

        [TestMethod]
        public void DeleteTest()
        {
            ObjectMappingForTest item = repository.GetById(2);
            Assert.IsNotNull(item, "Не найден объект по ID.");

            repository.Delete(2);
            context.SaveChanges();
            item = repository.GetById(2);
            Assert.IsNull(item, "Не удален объект в контекста.");

        }
    }
}