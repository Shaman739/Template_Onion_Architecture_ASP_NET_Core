using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Infrastructure;
using Shamdev.TOA.Web.Cache;
using Shamdev.TOA.WEB.Cache;
using Shamdev.TOA.WEB.Cache.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.Web.Cache
{
    [TestClass]
    public class ProcessingObjectCacheDecoratorTest
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
            _uow.SaveChangesAsync();
        }

        [TestMethod,Description("Проверка получения данных из БД, если нет в кеше")]
        public void CheckWorkWithCacheTest()
        {
            //ARRANGE
            MemoryCacheRepository<ObjectMappingForTest> cacheTestClass = new MemoryCacheRepository<ObjectMappingForTest>();
            DefaultCRUDBLL<ObjectMappingForTest> processingObjectTestCalss = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            FetchDomainData<ObjectMappingForTest> fetchDomainDataTestClass = new FetchDomainData<ObjectMappingForTest>(_uow);


            //ACT
            CacheCRUDBLLDecorator<ObjectMappingForTest> processingObjectCacheDecorator = new CacheCRUDBLLDecorator<ObjectMappingForTest>(processingObjectTestCalss, cacheTestClass);
            BaseResultType<ObjectMappingForTest> resultFromCache = cacheTestClass.GetByIdAsync(1).Result;
            BaseResultType<ObjectMappingForTest> resultFromDB = fetchDomainDataTestClass.GetByIdAsync(1).Result;
         
            
            //ASSERT
            Assert.IsNull(resultFromCache.Data);//В "БД" есть запись, но в кеше нет ее;
            Assert.AreEqual(ResultStatus.Fail, resultFromCache.Status);

            Assert.IsNotNull(resultFromDB.Data);//В "БД" есть запись
            Assert.AreEqual(ResultStatus.Success, resultFromDB.Status);
        }

        [TestMethod, Description("Проверка добавления данных в БД, если нет в кеше. Добавляет в кеш данные новые.")]
        public void AddSubscribeTest()
        {
            //ARRANGE
            MemoryCacheRepository<ObjectMappingForTest> cacheTestClass = new MemoryCacheRepository<ObjectMappingForTest>();
            DefaultCRUDBLL<ObjectMappingForTest> processingObjectTestCalss = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            FetchDomainData<ObjectMappingForTest> fetchDomainDataTestClass = new FetchDomainData<ObjectMappingForTest>(_uow);

            //ACT
            CacheCRUDBLLDecorator<ObjectMappingForTest> processingObjectCacheDecorator = new CacheCRUDBLLDecorator<ObjectMappingForTest>(processingObjectTestCalss, cacheTestClass);
            var resultSave=processingObjectCacheDecorator.SaveItemAsync(ExecuteTypeConstCRUD.ADD, new DefaultParamOfCRUDOperation<ObjectMappingForTest>() { Item = new ObjectMappingForTest() { IntValue = 22 ,IntValue2=33} }).Result;


            BaseResultType<ObjectMappingForTest> resultFromCache = cacheTestClass.GetByIdAsync(2).Result;
            BaseResultType<ObjectMappingForTest> resultFromDB = fetchDomainDataTestClass.GetByIdAsync(2).Result;


            //ASSERT
            Assert.IsNotNull(resultFromCache.Data);//В кеше есть данные
            Assert.AreEqual(ResultStatus.Success, resultFromCache.Status);

            Assert.AreEqual(resultFromDB.Data.IntValue , resultFromCache.Data.IntValue);//Проверка, что данные добавленные одинаковые
            Assert.AreEqual(resultFromDB.Data.IntValue2, resultFromCache.Data.IntValue2);

            //Удаляем для дальнейших тестов
            resultSave = processingObjectCacheDecorator.SaveItemAsync(ExecuteTypeConstCRUD.DELETE, new DefaultParamOfCRUDOperation<ObjectMappingForTest>() { Item = new ObjectMappingForTest() {Id=2, IntValue = 22, IntValue2 = 33 } }).Result;
            Assert.AreEqual(resultSave.Status, ResultStatus.Success);

        }

        [TestMethod, Description("Проверка изменения данных в БД, если нет данных в кеше. В кеше данные должны изменяться")]
        public void EditSubscribeTest()
        {
            //ARRANGE
            MemoryCacheRepository<ObjectMappingForTest> cacheTestClass = new MemoryCacheRepository<ObjectMappingForTest>();
            DefaultCRUDBLL<ObjectMappingForTest> processingObjectTestCalss = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            FetchDomainData<ObjectMappingForTest> fetchDomainDataTestClass = new FetchDomainData<ObjectMappingForTest>(_uow);

            //ACT
            CacheCRUDBLLDecorator<ObjectMappingForTest> processingObjectCacheDecorator = new CacheCRUDBLLDecorator<ObjectMappingForTest>(processingObjectTestCalss, cacheTestClass);
            var resultSave = processingObjectCacheDecorator.SaveItemAsync(ExecuteTypeConstCRUD.EDIT, new DefaultParamOfCRUDOperation<ObjectMappingForTest>() { Item = new ObjectMappingForTest() {Id=1, IntValue = 22, IntValue2 = 33 } }).Result;


            BaseResultType<ObjectMappingForTest> resultFromCache = cacheTestClass.GetByIdAsync(1).Result;
            BaseResultType<ObjectMappingForTest> resultFromDB = fetchDomainDataTestClass.GetByIdAsync(1).Result;


            //ASSERT
            Assert.IsNotNull(resultFromCache.Data);//В кеше есть данные
            Assert.AreEqual(ResultStatus.Success, resultFromCache.Status);

            Assert.AreEqual(resultFromDB.Data.IntValue, resultFromCache.Data.IntValue);//Проверка, что данные добавленные одинаковые
            Assert.AreEqual(resultFromDB.Data.IntValue2, resultFromCache.Data.IntValue2);
            Assert.AreEqual(33,resultFromDB.Data.IntValue2);

        }

        [TestMethod, Description("Проверка изменения данных в БД, если нет данных в кеше. В кеше данные должны изменяться")]
        public void EditInCacheSubscribeTest()
        {
            //ARRANGE
            MemoryCacheRepository<ObjectMappingForTest> cacheTestClass = new MemoryCacheRepository<ObjectMappingForTest>();
            DefaultCRUDBLL<ObjectMappingForTest> processingObjectTestCalss = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            FetchDomainData<ObjectMappingForTest> fetchDomainDataTestClass = new FetchDomainData<ObjectMappingForTest>(_uow);

            //ACT
            CacheCRUDBLLDecorator<ObjectMappingForTest> processingObjectCacheDecorator = new CacheCRUDBLLDecorator<ObjectMappingForTest>(processingObjectTestCalss, cacheTestClass);
            var resultSave = processingObjectCacheDecorator.SaveItemAsync(ExecuteTypeConstCRUD.EDIT, new DefaultParamOfCRUDOperation<ObjectMappingForTest>() { Item = new ObjectMappingForTest() { Id = 1, IntValue = 22, IntValue2 = 33 } }).Result;
            resultSave = processingObjectCacheDecorator.SaveItemAsync(ExecuteTypeConstCRUD.EDIT, new DefaultParamOfCRUDOperation<ObjectMappingForTest>() { Item = new ObjectMappingForTest() { Id = 1, IntValue = 22, IntValue2 = 44 } }).Result;

            BaseResultType<ObjectMappingForTest> resultFromCache = cacheTestClass.GetByIdAsync(1).Result;
            BaseResultType<ObjectMappingForTest> resultFromDB = fetchDomainDataTestClass.GetByIdAsync(1).Result;


            //ASSERT
            Assert.IsNotNull(resultFromCache.Data);//В кеше есть данные
            Assert.AreEqual(ResultStatus.Success, resultFromCache.Status);

            Assert.AreEqual(resultFromDB.Data.IntValue, resultFromCache.Data.IntValue);//Проверка, что данные добавленные одинаковые
            Assert.AreEqual(resultFromDB.Data.IntValue2, resultFromCache.Data.IntValue2);
            Assert.AreEqual(44, resultFromDB.Data.IntValue2);

        }

        [TestMethod, Description("Проверка удаления данных в БД и в кеше.")]
        public void DeleteInCacheAndDataBaseTest()
        {
            //ARRANGE
            MemoryCacheRepository<ObjectMappingForTest> cacheTestClass = new MemoryCacheRepository<ObjectMappingForTest>();
            DefaultCRUDBLL<ObjectMappingForTest> processingObjectTestCalss = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            FetchDomainData<ObjectMappingForTest> fetchDomainDataTestClass = new FetchDomainData<ObjectMappingForTest>(_uow);

            //ACT
            CacheCRUDBLLDecorator<ObjectMappingForTest> processingObjectCacheDecorator = new CacheCRUDBLLDecorator<ObjectMappingForTest>(processingObjectTestCalss, cacheTestClass);
            var resultSave = processingObjectCacheDecorator.SaveItemAsync(ExecuteTypeConstCRUD.ADD, new DefaultParamOfCRUDOperation<ObjectMappingForTest>() { Item = new ObjectMappingForTest() { IntValue = 2222, IntValue2 = 3333 } }).Result;
           

            BaseResultType<ObjectMappingForTest> resultFromCache = cacheTestClass.GetByIdAsync(3).Result;
            BaseResultType<ObjectMappingForTest> resultFromDB = fetchDomainDataTestClass.GetByIdAsync(3).Result;

            //ASSERT
            Assert.IsNotNull(resultFromCache.Data);//В кеше есть данные
            Assert.AreEqual(ResultStatus.Success, resultFromCache.Status);

            Assert.AreEqual(resultFromDB.Data.IntValue, resultFromCache.Data.IntValue);//Проверка, что данные добавленные одинаковые
            Assert.AreEqual(resultFromDB.Data.IntValue2, resultFromCache.Data.IntValue2);
            Assert.AreEqual(3333, resultFromDB.Data.IntValue2);



            //ACT - Удаляем данные
            resultSave = processingObjectCacheDecorator.SaveItemAsync(ExecuteTypeConstCRUD.DELETE, new DefaultParamOfCRUDOperation<ObjectMappingForTest>() { Item = new ObjectMappingForTest() { Id = 3 } }).Result;
            resultFromCache = cacheTestClass.GetByIdAsync(3).Result;
            resultFromDB = fetchDomainDataTestClass.GetByIdAsync(3).Result;
            //ASSERT
            Assert.IsNull(resultFromCache.Data);//В кеше есть данные
            Assert.AreEqual(ResultStatus.Fail, resultFromCache.Status);

            Assert.IsNull(resultFromDB.Data);//В кеше есть данные
            Assert.AreEqual(ResultStatus.Fail, resultFromDB.Status);

        }
    }
}
