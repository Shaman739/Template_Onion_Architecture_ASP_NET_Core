using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.ERP.Core.Data.Domain;
using Shamdev.ERP.DAL.Common.Repository;
using Shamdev.TOA.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.DAL.Repository
{
    [TestClass]
    public class UserRepositoryTest
    {

        [TestMethod, Description("Проверка выборки пользователя из БД при авторизации")]
        [DataTestMethod]
        [DataRow("a@mail.ru", "1", false)]
        [DataRow("123@mail.ru", "pass", true)]
        public void CheckIssueUserAsyncTest(string email, string password, bool finded)
        {
            //ARRANGE
            UserRepository userRepository = new UserRepository(context);

            //ACT
            User user = userRepository.CheckIssueUserAsync(email, password).Result;
            //ASSERT
            Assert.AreEqual(user != null, finded);
        }

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
            context.Add(new User() { Id = 1, Email = "123@mail.ru", Password = "pass" });
            context.Add(new User() { Id = 2, Email = "123l.ru", Password = "pass" });
            _uow = new UnitOfWork(context);
            _uow.SaveChangesAsync();
        }
    }
}
