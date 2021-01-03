using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL.ValidateContext;
using System;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.DAL.ValidateContext
{
    [TestClass]
    public class ValidateItemInContextTest
    {
        [TestMethod]
        public void ValidateTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationContextForTest>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            ApplicationContextForTest context = new ApplicationContextForTest(options);
            ValidateItemInContext<ObjectMappingForTest> validateItemInContext = new ValidateItemInContext<ObjectMappingForTest>(context);

            ObjectMappingForTest item = new ObjectMappingForTest();

            //Объекта нет в контексте. 
            ValidateContextResult validateContextResult = validateItemInContext.Validate(item);
            Assert.AreEqual(ResultStatus.Fail, validateContextResult.Status);
            Assert.AreEqual("Объект не найден в контексте для проверки обязательных полей", validateContextResult.Message);

            //Добавили в контекст, но с незаполненными обязательными полями
            context.Set<ObjectMappingForTest>().Add(item);
            validateContextResult = validateItemInContext.Validate(item);
            Assert.AreEqual(ResultStatus.Fail, validateContextResult.Status);
            Assert.AreEqual(1, validateContextResult.Count);
            Assert.AreEqual(2, validateContextResult[0].Fields.Count);
            Assert.AreEqual("IntValue", validateContextResult[0].Fields[0].Name);
            Assert.AreEqual("Строка", validateContextResult[0].Fields[1].Name);

            //Заполнили обязательные поля, чтобы прошла валидация
            item.IntValue = 1;
            item.IntValue2 = 2;
            validateContextResult = validateItemInContext.Validate(item);
            Assert.AreEqual(ResultStatus.Success, validateContextResult.Status);
            Assert.AreEqual(0, validateContextResult.Count);
        }
    }
}
