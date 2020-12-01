using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Infrastructure;
using Shamdev.TOA.DAL.Interface;
using System;
using System.Linq;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.BLL
{
    [TestClass]
    public class DefaultCRUDBLLTest
    {

        #region TestedFakeClass

        class PrepareItemForCRUDStrategyFake : IPrepareItemForCRUDStrategy<ObjectMappingForTest>
        {
            public ObjectMappingForTest GetItem(ObjectMappingForTest item)
            {
                throw new NotImplementedException("Для проверки ошибки подготовки в БЛЛ");
            }
        }

        class DefaultCRUDBLLForTest : DefaultCRUDBLL<ObjectMappingForTest>
        {
            public DefaultCRUDBLLForTest(IUnitOfWork contextDB) : base(contextDB)
            {
                PrepareItemForCRUDStrategyFactory.Value.ReplaceStrategy(ExecuteTypeConstCRUD.ADD, new PrepareItemForCRUDStrategyFake());
            }
        }
        #endregion
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
        public void SaveItemTest()
        {
            DefaultCRUDBLL<ObjectMappingForTest> bll = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItem(ExecuteTypeConstCRUD.ADD, null);
            Assert.IsFalse(resultCRUDOpeartion.IsSuccess);
            StringAssert.Contains(resultCRUDOpeartion.Message, "Объект для добавления/изменения не может быть null.");

            resultCRUDOpeartion = bll.SaveItem(ExecuteTypeConstCRUD.ADD, new DefaultParamOfCRUDOperation<ObjectMappingForTest>());
            Assert.IsFalse(resultCRUDOpeartion.IsSuccess);
            StringAssert.Contains(resultCRUDOpeartion.Message, "Объект для добавления/изменения не может быть null.");

            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { IntValue = 11, StrValue = "22" };

            Assert.AreEqual(1, bll.FetchDataAsync(new FetchDataParameters()).Result.TotalCountRows);
            resultCRUDOpeartion = bll.SaveItem(ExecuteTypeConstCRUD.ADD, objectFroCRUD);

            Assert.IsFalse(resultCRUDOpeartion.IsSuccess, "У объекта не заполнено обязательное поле IntValue2. Такое не должно проходить валидацию по контексту");

            objectFroCRUD.Item.IntValue2 = 33;
            //Пересоздаем контекст, так как были в него добавления других объектов, которые не сохранились.
            CreateContext();
            bll = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            resultCRUDOpeartion = bll.SaveItem(ExecuteTypeConstCRUD.ADD, objectFroCRUD);
            Assert.IsTrue(resultCRUDOpeartion.IsSuccess);
            Assert.IsNotNull(resultCRUDOpeartion.Data, "После сохранения должен отдаваться сохраненный объект.");
            Assert.IsInstanceOfType(resultCRUDOpeartion.Data, typeof(SaveResultType<ObjectMappingForTest>));
            Assert.AreEqual(2, resultCRUDOpeartion.Data.Item.Id);
            Assert.AreEqual(11, ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).IntValue);
            Assert.AreEqual("22", ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).StrValue);
            ResultFetchData<ObjectMappingForTest> allDataInDB = bll.FetchDataAsync(new FetchDataParameters()).Result;
            Assert.AreEqual(2, allDataInDB.TotalCountRows, "Должна была добавиться одна запись в БД");
            Assert.AreEqual(2, allDataInDB.Items[1].Id);
            Assert.AreEqual(11, allDataInDB.Items[1].IntValue);
            Assert.AreEqual("22", allDataInDB.Items[1].StrValue);

            //Проверка на изменение
            objectFroCRUD.Item = new ObjectMappingForTest() { Id = 1, IntValue = 111, StrValue = "222", IntValue2 = 222 };
            resultCRUDOpeartion = bll.SaveItem(ExecuteTypeConstCRUD.EDIT, objectFroCRUD);
            Assert.IsTrue(resultCRUDOpeartion.IsSuccess);
            Assert.IsNotNull(resultCRUDOpeartion.Data, "После сохранения должен отдаваться сохраненный объект.");
            Assert.IsInstanceOfType(resultCRUDOpeartion.Data, typeof(SaveResultType<ObjectMappingForTest>));
            Assert.AreEqual(1, resultCRUDOpeartion.Data.Item.Id);
            Assert.AreEqual(111, ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).IntValue);
            Assert.AreEqual("222", ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).StrValue);
            allDataInDB = bll.FetchDataAsync(new FetchDataParameters()).Result;
            Assert.AreEqual(2, allDataInDB.TotalCountRows, "Количество записей не должно меняться в БД");
            //Проверка, что данные изменились
            Assert.AreEqual(1, allDataInDB.Items[0].Id);
            Assert.AreEqual(111, allDataInDB.Items[0].IntValue);
            Assert.AreEqual("222", allDataInDB.Items[0].StrValue);

        }


        [TestMethod]
        public void SaveItemTest_Delete()
        {
            CreateContext();
            DefaultCRUDBLL<ObjectMappingForTest> bll = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);

            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            //Проверка на удаление не существующего объекта
            objectFroCRUD.Item = new ObjectMappingForTest() { Id = 10 };
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItem(ExecuteTypeConstCRUD.DELETE, objectFroCRUD);
            Assert.IsFalse(resultCRUDOpeartion.IsSuccess);
            Assert.IsNotNull(resultCRUDOpeartion.Data, "Результат не может быть null.");
            Assert.IsNull(resultCRUDOpeartion.Data.Item, "Объекта с таким ID нет в БД.");
            Assert.AreEqual(1, bll.FetchDataAsync(new FetchDataParameters()).Result.TotalCountRows, "Не должно измениться количество строк в БД");

            //Удаление объекта
            objectFroCRUD.Item = new ObjectMappingForTest() { Id = 1 };
            resultCRUDOpeartion = bll.SaveItem(ExecuteTypeConstCRUD.DELETE, objectFroCRUD);
            Assert.IsTrue(resultCRUDOpeartion.IsSuccess);
            Assert.IsNotNull(resultCRUDOpeartion.Data, "После сохранения должен отдаваться сохраненный объект.");
            Assert.IsInstanceOfType(resultCRUDOpeartion.Data, typeof(SaveResultType<ObjectMappingForTest>));
            Assert.AreEqual(0, bll.FetchDataAsync(new FetchDataParameters()).Result.TotalCountRows, "Не должно измениться количество строк в БД");
        }
        [TestMethod]
        public void SaveItemWithErrorTest()
        {
            // В этом БЛЛ будет ошибка, так как стратегия на добавления заменена на fake с ошибкой. Нужно проверить, что результат подготовки будет с ошибкой и не будет сохранений
            DefaultCRUDBLLForTest bll = new DefaultCRUDBLLForTest(_uow);
            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { StrValue = "222222" };
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItem(ExecuteTypeConstCRUD.ADD, objectFroCRUD);
            Assert.IsFalse(resultCRUDOpeartion.IsSuccess, "Не прошла валидация по обязательным полям контекста.");
            StringAssert.Contains("Для проверки ошибки подготовки в БЛЛ", resultCRUDOpeartion.Message, "Должно пробрасываться сообщение об ошибке из стратегии.");
            ResultFetchData<ObjectMappingForTest> allDataInDB = bll.FetchDataAsync(new FetchDataParameters()).Result;
            Assert.IsNull(allDataInDB.Items.FirstOrDefault(x => x.StrValue == "222222"), "Не должно быть записи в БД, так как ошибка подготовки была");
        }

        [TestMethod]
        public void ValidateTest()
        {
            DefaultCRUDBLL<ObjectMappingForTest> bll = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { StrValue = "22" };
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItem(ExecuteTypeConstCRUD.ADD, objectFroCRUD);
            Assert.IsFalse(resultCRUDOpeartion.IsSuccess, "Не прошла валидация по обязательным полям контекста.");
        }

        [TestMethod]
        public void GetByIdTest()
        {
            CreateContext();
            DefaultCRUDBLL<ObjectMappingForTest> bll = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);

            BaseResultType<ObjectMappingForTest> resultGetById = bll.GetByIdAsync(10).Result;
            Assert.IsFalse(resultGetById.IsSuccess);
            Assert.AreEqual("Запись не найдена.", ((BaseResultType<ObjectMappingForTest>)resultGetById).Message);

            //Проверка успешного получения записи

            resultGetById = bll.GetByIdAsync(1).Result;

            Assert.IsTrue(resultGetById.IsSuccess);
            Assert.IsNotNull(resultGetById.Data);
            Assert.AreEqual(1, resultGetById.Data.Id);
            Assert.AreEqual(2, resultGetById.Data.IntValue);
            Assert.AreEqual("2", resultGetById.Data.StrValue);
        }

        [TestMethod]
        public void SaveItem_IsOnlyAddInContext_Test()
        {
            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { IntValue = 11, StrValue = "22" };
            objectFroCRUD.Item.IntValue2 = 33;



            CreateContext();
            DefaultCRUDBLL<ObjectMappingForTest>  bll = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            bll.IsOnlyAddInContext = true;
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItem(ExecuteTypeConstCRUD.ADD, objectFroCRUD);
            Assert.IsTrue(resultCRUDOpeartion.IsSuccess);
            Assert.IsNotNull(resultCRUDOpeartion.Data, "После сохранения должен отдаваться сохраненный объект.");
            Assert.IsInstanceOfType(resultCRUDOpeartion.Data, typeof(SaveResultType<ObjectMappingForTest>));
            //Вернет объект, но не сохранит его в БД
            Assert.AreEqual(2, resultCRUDOpeartion.Data.Item.Id);
            Assert.AreEqual(11, ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).IntValue);
            Assert.AreEqual("22", ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).StrValue);
            ResultFetchData<ObjectMappingForTest> allDataInDB = bll.FetchDataAsync(new FetchDataParameters()).Result;
            Assert.AreEqual(1, allDataInDB.TotalCountRows, "Должна была добавиться одна запись в БД");
            Assert.AreEqual(1, allDataInDB.Items[0].Id);
            Assert.AreEqual(2, allDataInDB.Items[0].IntValue);
            Assert.AreEqual("2", allDataInDB.Items[0].StrValue);

          

        }
    }
}
