using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.ERP.Core.Data.Domain;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Service;
using Shamdev.TOA.BLL.Service.DTO;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTestProject.DAL.TestFakeClasses;
using UnitTestProject.Web.FakeClass;

namespace UnitTestProject.BLL.Service
{
    [TestClass]
    public class AccountServiceTest
    {
        [TestMethod]
        public void LoginAsyncTest_CheckContext()
        {

            //ASSERT
            Assert.ThrowsException<ArgumentNullException>(() => { new AccountService(null,null); });
        }

        [TestMethod]
        public void LoginAsyncTest_EmptyParam()
        {
            // ARRANGE
            AccountService accountService = new AccountService(_uow, new UserContextFake());
            
            // ACT
            BaseResultType result = accountService.LoginAllowCheckAsync(new DefaultParamOfCRUDOperation<UserDTO>()).Result;

            //ASSERT
            Assert.IsTrue(result.Status == ResultStatus.Fail);
        }

        [TestMethod]
        public void LoginAsyncTest_NullParam()
        {
            // ARRANGE
            AccountService accountService = new AccountService(_uow, new UserContextFake());

            // ACT
            BaseResultType result = accountService.LoginAllowCheckAsync(null).Result;

            //ASSERT
            Assert.IsTrue(result.Status == ResultStatus.Fail);
        }

        [TestMethod,Description("Проверка поиска пользователя в БД")]
        [DataTestMethod]
        [DataRow("a@mail.ru", "1", ResultStatus.Fail)]
        [DataRow("123@mail.ru", "pass", ResultStatus.Success)]
       // [DataRow("123l.ru", "pass", ResultStatus.Fail)] //TODO: Добавить проверку корректности почты
        public void LoginAsyncTest_CheckUser(string email,string password, ResultStatus resultCheck)
        {
            // ARRANGE
            AccountService accountService = new AccountService(_uow, new UserContextFake());
            DefaultParamOfCRUDOperation<UserDTO> userParam = new DefaultParamOfCRUDOperation<UserDTO>();
            userParam.Item = new UserDTO();
            userParam.Item.Email = email;
            userParam.Item.Password = password;
          
            // ACT
            BaseResultType result = accountService.LoginAllowCheckAsync(userParam).Result;
          
            //ASSERT
            Assert.IsTrue(result.Status == resultCheck);
        }
        [TestMethod, Description("Регистрация пользователя")]
        public void RegisterAsyncTest() 
        {
            // ARRANGE
            AccountService accountService = new AccountService(_uow, new UserContextFake());
            DefaultParamOfCRUDOperation<UserDTO> userParam = new DefaultParamOfCRUDOperation<UserDTO>();
            userParam.Item = new UserDTO();
            userParam.Item.Email = "a@ma1il.ru";
            userParam.Item.Password = "1";
            int countUsersInDB = context.Users.CountAsync().Result;

            // ACT
            BaseResultType result = accountService.RegisterAsync(userParam).Result;

            //ASSERT
            Assert.IsTrue(result.Status == ResultStatus.Success);
            Assert.IsTrue(context.Users.CountAsync().Result == countUsersInDB+1);
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

        private async static void CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContextForTest>()
                                          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                         .Options;
            context = new ApplicationContextForTest(options);
            context.Add(new User() { Id = 1, Email = "123@mail.ru", Password = "pass" });
            context.Add(new User() { Id = 2, Email = "123l.ru", Password = "pass" });
            _uow = new UnitOfWork(context);
            await _uow.SaveChangesAsync();
        }
    }
}
