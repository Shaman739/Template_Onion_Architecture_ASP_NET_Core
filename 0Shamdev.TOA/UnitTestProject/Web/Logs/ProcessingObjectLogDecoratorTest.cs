using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.MongoDB.Interface;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.Core.Data.MongoDB;
using Shamdev.TOA.DAL;
using Shamdev.TOA.Web.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.Web.Logs
{
    [TestClass]
    public class ProcessingObjectLogDecoratorTest
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
            context.Add(new ObjectMappingForTest() { Id = 2, IntValue = 2, StrValue = "2", SubObject = new SubObjectMappingForTest() { Id = 34, IntValueSub = 34, StrValueSub = "35" } });
            _uow = new UnitOfWork(context);
            _uow.SaveChangesAsync();
        }

        class LogTest : ILog
        {
            public List<LogItem> LogItemsCollection { get; set; }
            public LogTest()
            {
                LogItemsCollection = new List<LogItem>();
            }
            public Task AddLog(LogItem log)
            {
                LogItemsCollection.Add(log);
                return Task.Run(() =>
                {

                    
                });
            }

            public int GetCount()
            {
                  return LogItemsCollection.Count;
            }
        }
        [TestMethod, Description("При любом событии(добавлении) должна быть запись в лог.")]
        public void AddLogAddEventTest()
        {
            AddLogTest(ExecuteTypeConstCRUD.ADD,null);
        }

        [TestMethod, Description("При любом событии( изменение) должна быть запись в лог.")]
        public void AddLogEditEventTest()
        {
            AddLogTest(ExecuteTypeConstCRUD.EDIT, 1);
        }
        [TestMethod, Description("При любом событии( удаление) должна быть запись в лог.")]
        public void AddLogDeleteEventTest()
        {
            AddLogTest(ExecuteTypeConstCRUD.DELETE, 2);
        }


        public void AddLogTest(ExecuteTypeConstCRUD executeTypeConstCRUD,long? id)
        {
            CreateContext();
            //ARRANGE
            LogTest logTestClass = new LogTest();
            Assert.AreEqual(0, logTestClass.LogItemsCollection.Count);//Пустые логи
            DefaultCRUDBLL<ObjectMappingForTest> processingObjectTestCalss = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            ObjectMappingForTest item = new ObjectMappingForTest() { IntValue = 22, IntValue2 = 33 };
            if (id !=null)
                item.Id = (long)id;
            //ACT

            LogerCRUDBLLDecoratorDecorator<ObjectMappingForTest> processingObjectLogDecorator = new LogerCRUDBLLDecoratorDecorator<ObjectMappingForTest>(processingObjectTestCalss, logTestClass);
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultSave = processingObjectLogDecorator.SaveItemAsync(executeTypeConstCRUD, new DefaultParamOfCRUDOperation<ObjectMappingForTest>() { Item = item }).Result;
            
            //ASSERT
            Assert.AreEqual(ResultStatus.Success, resultSave.Status);
            Assert.AreEqual(1, logTestClass.GetCount());//Лог добавился
        }


    }
}
