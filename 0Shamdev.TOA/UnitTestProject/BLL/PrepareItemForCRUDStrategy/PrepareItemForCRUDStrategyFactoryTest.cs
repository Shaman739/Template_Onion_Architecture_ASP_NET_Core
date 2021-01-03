using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.PrepareItemForCRUDOperations;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL;
using System;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.BLL.PrepareItemForCRUDStrategy
{
    [TestClass]
    public class PrepareItemForCRUDStrategyFactoryTest
    {
        static UnitOfWork _uow;
        [TestInitialize]
        public void Init()
        {
            if (_uow == null)
            {
                var options = new DbContextOptionsBuilder<ApplicationContextForTest>()
                              .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                             .Options;
                ApplicationContextForTest context = new ApplicationContextForTest(options);
                context.Add(new ObjectMappingForTest() { Id = 1, IntValue = 2, StrValue = "2", SubObject = new SubObjectMappingForTest() { Id = 33, IntValueSub = 34, StrValueSub = "35" } });
                _uow = new UnitOfWork(context);
                _uow.SaveChanges();
            }
        }
        [TestMethod]
        public void GetStrategyTest()
        {
            Assert.IsInstanceOfType(new PrepareItemForCRUDStrategyFactory<ObjectMappingForTest>(_uow).GetStrategy(ExecuteTypeConstCRUD.ADD), typeof(AddPrepareItemForCRUDStrategy<ObjectMappingForTest>));
            Assert.IsInstanceOfType(new PrepareItemForCRUDStrategyFactory<ObjectMappingForTest>(_uow).GetStrategy(ExecuteTypeConstCRUD.EDIT), typeof(EditPrepareItemForCRUDStrategy<ObjectMappingForTest>));
            Assert.IsInstanceOfType(new PrepareItemForCRUDStrategyFactory<ObjectMappingForTest>(_uow).GetStrategy(ExecuteTypeConstCRUD.DELETE), typeof(DeletePrepareItemForCRUDStrategy<ObjectMappingForTest>));
        }

        [TestMethod]
        public void DeleteStrategy()
        {
            PrepareItemForCRUDStrategyFactory<ObjectMappingForTest> factory = new PrepareItemForCRUDStrategyFactory<ObjectMappingForTest>(_uow);
            Assert.IsNotNull(factory.GetStrategy(ExecuteTypeConstCRUD.ADD));
            factory.RemoveStrategy(ExecuteTypeConstCRUD.ADD);
            Assert.ThrowsException<Exception>(() => factory.GetStrategy(ExecuteTypeConstCRUD.ADD), "Ошибка, так как должно бытьисклчюение при поиске стратегии по типу");
        }

        [TestMethod]
        public void ReplaceStrategy()
        {
            PrepareItemForCRUDStrategyFactory<ObjectMappingForTest> factory = new PrepareItemForCRUDStrategyFactory<ObjectMappingForTest>(_uow);
            Assert.IsInstanceOfType(factory.GetStrategy(ExecuteTypeConstCRUD.ADD), typeof(AddPrepareItemForCRUDStrategy<ObjectMappingForTest>));
            factory.ReplaceStrategy(ExecuteTypeConstCRUD.ADD, new NewStrategy<ObjectMappingForTest>());
            Assert.IsInstanceOfType(factory.GetStrategy(ExecuteTypeConstCRUD.ADD), typeof(NewStrategy<ObjectMappingForTest>));

        }

        [TestMethod]
        public void OnlyADDStrategy()
        {
            PrepareItemForCRUDStrategyFactory<ObjectMappingForTest> factory = new PrepareItemForCRUDStrategyFactory<ObjectMappingForTest>(_uow);
            factory.RemoveStrategy(ExecuteTypeConstCRUD.ADD);
            Assert.ThrowsException<Exception>(() => factory.GetStrategy(ExecuteTypeConstCRUD.ADD), "Ошибка, так как должно бытьисклчюение при поиске стратегии по типу");

            factory.ReplaceStrategy(ExecuteTypeConstCRUD.ADD, new NewStrategy<ObjectMappingForTest>());
            Assert.IsInstanceOfType(factory.GetStrategy(ExecuteTypeConstCRUD.ADD), typeof(NewStrategy<ObjectMappingForTest>));
            Assert.IsNotNull(factory.GetStrategy(ExecuteTypeConstCRUD.ADD));
        }

        [TestMethod]
        public void PrepareItemTest()
        {
            PrepareItemForCRUDStrategyFactory<ObjectMappingForTest> factory = new PrepareItemForCRUDStrategyFactory<ObjectMappingForTest>(_uow);
            DefaultParamOfCRUDOperation<ObjectMappingForTest> sourceObjectMappingForTest = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            sourceObjectMappingForTest.Item =new ObjectMappingForTest();
            sourceObjectMappingForTest.Item.IntValue = 1;
            sourceObjectMappingForTest.Item.StrValue = "str";
            BaseResultType<PrepareItemResult<ObjectMappingForTest>> prepareItemResult = factory.PrepareItem(sourceObjectMappingForTest, ExecuteTypeConstCRUD.ADD);

            //Проверка при добавлении
            Assert.AreEqual(ResultStatus.Success, prepareItemResult.Status, "Подготовка для добавления в БД должна быть успешной");
            Assert.IsNotNull(prepareItemResult.Data);
            Assert.AreEqual(sourceObjectMappingForTest.Item.IntValue, prepareItemResult.Data.Item.IntValue);
            Assert.AreEqual(sourceObjectMappingForTest.Item.StrValue, prepareItemResult.Data.Item.StrValue);
            Assert.IsNull(prepareItemResult.Data.Item.SubObject, "Мапятся только простые типы.");
            Assert.IsTrue(String.IsNullOrWhiteSpace(prepareItemResult.Message));

            //Проверка при изменение с несуществующим объектом в БД
            sourceObjectMappingForTest.Item.Id = 1000;
            prepareItemResult = factory.PrepareItem(sourceObjectMappingForTest, ExecuteTypeConstCRUD.EDIT);
            Assert.AreEqual(ResultStatus.Fail, prepareItemResult.Status, "Подготовка для изменения в БД должна быть не успешной, так как не задан id, а для изменения происходит запрос объекта из БД.");
            Assert.AreEqual("Объект не найден в БД для изменения.", prepareItemResult.Message);

            //Проверка  с существующим БД при изменение
            sourceObjectMappingForTest.Item.Id = 1;
            prepareItemResult = factory.PrepareItem(sourceObjectMappingForTest, ExecuteTypeConstCRUD.EDIT);
            Assert.IsNotNull(prepareItemResult.Data);
            Assert.AreEqual(sourceObjectMappingForTest.Item.IntValue, prepareItemResult.Data.Item.IntValue);

            Assert.AreEqual(sourceObjectMappingForTest.Item.StrValue, prepareItemResult.Data.Item.StrValue);
            Assert.IsNotNull(prepareItemResult.Data.Item.SubObject, "В БД есть ссылка на этот объект и не должно затираться");
            Assert.AreEqual(33, prepareItemResult.Data.Item.SubObject.Id);
            Assert.AreEqual(34, prepareItemResult.Data.Item.SubObject.IntValueSub);
            Assert.AreEqual("35", prepareItemResult.Data.Item.SubObject.StrValueSub);

            Assert.IsTrue(String.IsNullOrWhiteSpace(prepareItemResult.Message));


        }

    }

    internal class NewStrategy<T> : IPrepareItemForCRUDStrategy<ObjectMappingForTest>
    {
        public NewStrategy()
        {
        }

        public ObjectMappingForTest GetItem(DefaultParamOfCRUDOperation<ObjectMappingForTest> item)
        {
            return new ObjectMappingForTest();
        }
    }
}
