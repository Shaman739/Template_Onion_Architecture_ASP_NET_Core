﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.BLL;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.BLL
{
    [TestClass]
    public class FetchDomainDataTest
    {
        static UnitOfWork _uow;
        static ApplicationContextForTest context;
        [TestInitialize]
        public void Init()
        {
            if (_uow == null)
            {
                CreateContext();
            }
        }

        private static void CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContextForTest>()
                                          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                         .Options;
            context = new ApplicationContextForTest(options);
            context.Add(new ObjectMappingForTest() { Id = 1, IntValue = 2, StrValue = "2", SubObject = new SubObjectMappingForTest() { Id = 33, IntValueSub = 34, StrValueSub = "35" } });
            _uow = new UnitOfWork(context);
            _uow.SaveChanges();
        }


        [TestMethod]
        public void GetByIdTest()
        {
            CreateContext();
            FetchDomainData<ObjectMappingForTest> bll = new FetchDomainData<ObjectMappingForTest>(_uow);

            BaseResultType<ObjectMappingForTest> resultGetById = bll.GetByIdAsync(10).Result;
            Assert.AreEqual(ResultStatus.Fail, resultGetById.Status);
            Assert.AreEqual("Запись не найдена.", ((BaseResultType<ObjectMappingForTest>)resultGetById).Message);

            //Проверка успешного получения записи

            resultGetById = bll.GetByIdAsync(1).Result;

            Assert.AreEqual(ResultStatus.Success, resultGetById.Status);
            Assert.IsNotNull(resultGetById.Data);
            Assert.AreEqual(1, resultGetById.Data.Id);
            Assert.AreEqual(2, resultGetById.Data.IntValue);
            Assert.AreEqual("2", resultGetById.Data.StrValue);
        }
    }
}
