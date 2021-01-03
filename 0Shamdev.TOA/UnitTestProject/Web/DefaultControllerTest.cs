using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Infrastructure;
using Shamdev.TOA.Web;
using Shamdev.TOA.Web.Infrastructure.TypeOfResultQuery;
using System;
using System.Linq;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.Web
{
    [TestClass]
    public class DefaultControllerTest
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
            context.Add(new ObjectMappingForTest() { Id = 1, IntValue = 2, StrValue = "2", SubObject = new SubObjectMappingForTest() { Id = 33, IntValueSub = 33, StrValueSub = "33" } });
            context.Add(new ObjectMappingForTest() { Id = 2, IntValue = 22, StrValue = "23", SubObject = new SubObjectMappingForTest() { Id = 34, IntValueSub = 34, StrValueSub = "34" } });
            context.Add(new ObjectMappingForTest() { Id = 3, IntValue = 23, StrValue = "23", SubObject = new SubObjectMappingForTest() { Id = 35, IntValueSub = 35, StrValueSub = "35" } });
            _uow = new UnitOfWork(context);
            _uow.SaveChanges();
        }

        class FakeLogger : ILogger<DefaultController<ObjectMappingForTest>>
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                throw new NotImplementedException();
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                throw new NotImplementedException();
            }
        }
        [TestMethod]
        public void GetAsyncTest()
        {
            CreateContext();
            DefaultController<ObjectMappingForTest> defaultController = new DefaultController<ObjectMappingForTest>(new FakeLogger(), new DefaultCRUDBLL<ObjectMappingForTest>(_uow));

            //Выборка первой страницы с дефолтным размером стриницы
            JsonResult result = defaultController.GetAsync(null).Result;
            BaseResultType<ResultFetchData<ObjectMappingForTest>> fetchDataResultQuery = (BaseResultType<ResultFetchData<ObjectMappingForTest>>)result.Value;
            Assert.AreEqual(ResultStatus.Success, fetchDataResultQuery.Status);
            Assert.AreEqual(3, fetchDataResultQuery.Data.TotalCountRows);
            Assert.AreEqual(3, fetchDataResultQuery.Data.Items.Count);

            //Проверка выборки первой страницы 
            result = defaultController.GetAsync(new FetchDataParameters() { CountOnPage = 1 }).Result;
            fetchDataResultQuery = (BaseResultType<ResultFetchData<ObjectMappingForTest>>)result.Value;
            Assert.AreEqual(ResultStatus.Success,fetchDataResultQuery.Status);
            Assert.AreEqual(3, fetchDataResultQuery.Data.TotalCountRows, "Общий подсчет записей при запросе не работает.");
            Assert.AreEqual(1, fetchDataResultQuery.Data.Items.Count);
            Assert.AreEqual(1, fetchDataResultQuery.Data.Items[0].Id);
            Assert.AreEqual(1, fetchDataResultQuery.Data.PageNumber);
            //Проверка выборки второй страницы 
            result = defaultController.GetAsync(new FetchDataParameters() { CountOnPage = 1, PageNumber = 2 }).Result;
            fetchDataResultQuery = (BaseResultType<ResultFetchData<ObjectMappingForTest>>)result.Value;
            Assert.AreEqual(ResultStatus.Success, fetchDataResultQuery.Status);
            Assert.AreEqual(3, fetchDataResultQuery.Data.TotalCountRows);
            Assert.AreEqual(1, fetchDataResultQuery.Data.Items.Count);
            Assert.AreEqual(2, fetchDataResultQuery.Data.Items[0].Id);
            Assert.AreEqual(2, fetchDataResultQuery.Data.PageNumber);

        }

        [TestMethod]
        public void AddTest()
        {
            CreateContext();
            DefaultController<ObjectMappingForTest> defaultController = new DefaultController<ObjectMappingForTest>(new FakeLogger(), new DefaultCRUDBLL<ObjectMappingForTest>(_uow));

            //Проверка на возврат ошибки
            JsonResult resultAdd = defaultController.Add(new DefaultParamOfCRUDOperation<ObjectMappingForTest>()).Result;
            BaseResultType resultQuery = (FailResultQuery)resultAdd.Value;
            Assert.AreEqual(ResultStatus.Fail, resultQuery.Status);
            Assert.AreEqual("Объект для добавления/изменения не может быть null.", ((FailResultQuery)resultQuery).Message);

            DefaultParamOfCRUDOperation<ObjectMappingForTest> paramQueryAdd = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            paramQueryAdd.Item = new ObjectMappingForTest() { IntValue = 1, IntValue2 = 1, StrValue = "1" };
            resultAdd = defaultController.Add(paramQueryAdd).Result;
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultSuccessQuery = ((BaseResultType<SaveResultType<ObjectMappingForTest>>)resultAdd.Value);
            Assert.AreEqual(ResultStatus.Success, resultSuccessQuery.Status);
            Assert.IsNotNull(resultSuccessQuery.Data);

            Assert.IsInstanceOfType(resultSuccessQuery.Data, typeof(SaveResultType<ObjectMappingForTest>), "Вернулся тип объекта не ObjectMappingForTest после сохранения этого объекта.");
            Assert.AreEqual(1, ((ObjectMappingForTest)resultSuccessQuery.Data.Item).IntValue);
            Assert.AreEqual(1, ((ObjectMappingForTest)resultSuccessQuery.Data.Item).IntValue2);
            Assert.AreEqual("1", ((ObjectMappingForTest)resultSuccessQuery.Data.Item).StrValue);
            Assert.AreEqual(4, ((ObjectMappingForTest)resultSuccessQuery.Data.Item).Id);

            //Проверка запроса из БД, что запись добавилась
            JsonResult result = defaultController.GetAsync(new FetchDataParameters() { CountOnPage = 10, PageNumber = 1 }).Result;
            BaseResultType<ResultFetchData<ObjectMappingForTest>> fetchDataResultQuery = (BaseResultType<ResultFetchData<ObjectMappingForTest>>)result.Value;
            Assert.AreEqual(ResultStatus.Success, fetchDataResultQuery.Status);
            Assert.AreEqual(4, fetchDataResultQuery.Data.TotalCountRows);
        }

        [TestMethod]
        public void EditTest()
        {
            CreateContext();
            DefaultController<ObjectMappingForTest> defaultController = new DefaultController<ObjectMappingForTest>(new FakeLogger(), new DefaultCRUDBLL<ObjectMappingForTest>(_uow));

            //Проверка на возврат ошибки
            JsonResult resultAdd = defaultController.Edit(new DefaultParamOfCRUDOperation<ObjectMappingForTest>()).Result;
            BaseResultType resultQuery = (FailResultQuery)resultAdd.Value;
            Assert.AreEqual(ResultStatus.Fail, resultQuery.Status);
            Assert.AreEqual("Объект для добавления/изменения не может быть null.", ((FailResultQuery)resultQuery).Message);

            //Проверка редактирования без ID.Будет ошибка, так как нет идентификатора записи
            DefaultParamOfCRUDOperation<ObjectMappingForTest> paramQueryAdd = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            paramQueryAdd.Item = new ObjectMappingForTest()
            {
                IntValue = 1,
                IntValue2 = 1,
                StrValue = "1"
            };
            resultAdd = defaultController.Edit(paramQueryAdd).Result;
            resultQuery = (FailResultQuery)resultAdd.Value;
            Assert.AreEqual(ResultStatus.Fail, resultQuery.Status);
            Assert.AreEqual("Объект не найден в БД для изменения.", ((FailResultQuery)resultQuery).Message);

            //Проверка успешного изменения записи
            paramQueryAdd.Item = new ObjectMappingForTest()
            {
                Id = 3,
                IntValue = 1,
                IntValue2 = 1,
                StrValue = "1"
            };
            resultAdd = defaultController.Edit(paramQueryAdd).Result;
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultSuccessQuery = (BaseResultType<SaveResultType<ObjectMappingForTest>>)resultAdd.Value;
            Assert.AreEqual(ResultStatus.Success, resultSuccessQuery.Status);
            Assert.IsNotNull(resultSuccessQuery.Data.Item);

            Assert.IsInstanceOfType(resultSuccessQuery.Data.Item, typeof(ObjectMappingForTest), "Вернулся тип объекта не ObjectMappingForTest после сохранения этого объекта.");
            Assert.AreEqual(1, ((ObjectMappingForTest)resultSuccessQuery.Data.Item).IntValue);
            Assert.AreEqual(1, ((ObjectMappingForTest)resultSuccessQuery.Data.Item).IntValue2);
            Assert.AreEqual("1", ((ObjectMappingForTest)resultSuccessQuery.Data.Item).StrValue);
            Assert.AreEqual(3, ((ObjectMappingForTest)resultSuccessQuery.Data.Item).Id);


            //Проверка запроса из БД, что запись не добавилась
            JsonResult result = defaultController.GetAsync(new FetchDataParameters() { CountOnPage = 10, PageNumber = 1 }).Result;
            BaseResultType<ResultFetchData<ObjectMappingForTest>> fetchDataResultQuery = (BaseResultType<ResultFetchData<ObjectMappingForTest>>)result.Value;
            Assert.AreEqual(ResultStatus.Success, fetchDataResultQuery.Status);
            Assert.AreEqual(3, fetchDataResultQuery.Data.TotalCountRows);

            ObjectMappingForTest itemFromDB = fetchDataResultQuery.Data.Items[2];
            Assert.AreEqual(1, itemFromDB.IntValue);
            Assert.AreEqual(1, itemFromDB.IntValue2);
            Assert.AreEqual("1", itemFromDB.StrValue);
            Assert.AreEqual(3, itemFromDB.Id);
        }
        [TestMethod]
        public void DeleteTest()
        {
            CreateContext();
            DefaultController<ObjectMappingForTest> defaultController = new DefaultController<ObjectMappingForTest>(new FakeLogger(), new DefaultCRUDBLL<ObjectMappingForTest>(_uow));

            JsonResult resultAdd = defaultController.Delete(10).Result;
            BaseResultType resultQuery = (FailResultQuery)resultAdd.Value;
            Assert.AreEqual(ResultStatus.Fail, resultQuery.Status);
            Assert.AreEqual("Записи для удаления не существует.", ((FailResultQuery)resultQuery).Message);

            //Проверка успешного удаления записи
            long idDelete = 3;
            resultAdd = defaultController.Delete(idDelete).Result;
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultSuccessQuery = (BaseResultType<SaveResultType<ObjectMappingForTest>>)resultAdd.Value;
            Assert.AreEqual(ResultStatus.Success, resultSuccessQuery.Status);
            Assert.IsNotNull(resultSuccessQuery.Data.Item);

            //Проверка запроса из БД, что запись удалилась
            JsonResult result = defaultController.GetAsync(new FetchDataParameters() { CountOnPage = 10, PageNumber = 1 }).Result;
            BaseResultType<ResultFetchData<ObjectMappingForTest>> fetchDataResultQuery = (BaseResultType<ResultFetchData<ObjectMappingForTest>>)result.Value;
            Assert.AreEqual(ResultStatus.Success, fetchDataResultQuery.Status);
            Assert.AreEqual(2, fetchDataResultQuery.Data.TotalCountRows);

            Assert.AreEqual(2, fetchDataResultQuery.Data.Items.Count);
            Assert.IsFalse(fetchDataResultQuery.Data.Items.Any(x => x.Id == idDelete));

        }

        [TestMethod]
        public void GetByIdTest()
        {
            CreateContext();
            DefaultController<ObjectMappingForTest> defaultController = new DefaultController<ObjectMappingForTest>(new FakeLogger(), new DefaultCRUDBLL<ObjectMappingForTest>(_uow));

            JsonResult resultGetById = defaultController.GetByIdAsync(10).Result;
            BaseResultType<ObjectMappingForTest> resultQuery = (BaseResultType<ObjectMappingForTest>)resultGetById.Value;
            Assert.AreEqual(ResultStatus.Fail, resultQuery.Status);
            Assert.AreEqual("Запись не найдена.", resultQuery.Message);

            //Проверка успешного получения записи

            resultGetById = defaultController.GetByIdAsync(3).Result;
            BaseResultType<ObjectMappingForTest> resultSuccessQuery = (BaseResultType<ObjectMappingForTest>)resultGetById.Value;
            Assert.AreEqual(ResultStatus.Success, resultSuccessQuery.Status);
            Assert.IsNotNull(resultSuccessQuery.Data);
            Assert.AreEqual(3, resultSuccessQuery.Data.Id);
            Assert.AreEqual(23, resultSuccessQuery.Data.IntValue);
            Assert.AreEqual("23", resultSuccessQuery.Data.StrValue);



        }
    }
}
