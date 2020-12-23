using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Validate;
using Shamdev.TOA.BLL.Validate.Interface;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using Shamdev.TOA.DAL;
using Shamdev.TOA.DAL.Interface;
using System;
using UnitTestProject.DAL.TestFakeClasses;
using Shamdev.ERP.Core.Data.Infrastructure.ResultType.Question;
namespace UnitTestProject.BLL.Validate
{
    [TestClass]
    public class ValidateDomainObjectFactoryTest
    {
        class CustomValidateDomainObject : IValidateDomainObject<ObjectMappingForTest>
        {
            public BaseResultType Validate(ObjectMappingForTest item)
            {
                BaseResultType baseResultType = new BaseResultType() { IsSuccess = true };
                if (String.IsNullOrWhiteSpace(item.StrValue))
                    baseResultType.AddError("Пустое значение StrValue.");
                return baseResultType;
            }
        }
        class ValidateDomainObjectFactoryForTest : ValidateDomainObjectFactory<ObjectMappingForTest>
        {
            public ValidateDomainObjectFactoryForTest(IUnitOfWork contextDB) : base(contextDB)
            {
                AddStrategy(ValidateTypeConstCRUD.ADD_OR_EDIT, new Lazy<IValidateDomainObject<ObjectMappingForTest>>(() => new CustomValidateDomainObject()));
            }
        }

        class QuestionValidateDomainObject : IValidateDomainObject<ObjectMappingForTest>
        {
            public BaseResultType Validate(ObjectMappingForTest item)
            {
                BaseResultType baseResultType = new BaseResultType() { IsSuccess = true };
                if (String.IsNullOrWhiteSpace(item.StrValue))
                {
                    WarningQuestion question = new WarningQuestion()
                    {
                        Id = 1,
                        Message = "Предупреждение"
                    };
                    baseResultType.AddQuestion(question);
                }
                return baseResultType;
            }
        }
        class QuestionValidateFactoryForTest : ValidateDomainObjectFactory<ObjectMappingForTest>
        {
            public QuestionValidateFactoryForTest(IUnitOfWork contextDB) : base(contextDB)
            {
                AddStrategy(ValidateTypeConstCRUD.ADD_OR_EDIT, new Lazy<IValidateDomainObject<ObjectMappingForTest>>(() => new QuestionValidateDomainObject()));
            }
        }
        [TestMethod]
        public void GetValidateTest_AddOrEdit()
        {
            ApplicationContextForTest context;
            UnitOfWork uow;
            CreateContext(out context, out uow);

            ValidateDomainObjectFactory<ObjectMappingForTest> validateDomainObjectFactory = new ValidateDomainObjectFactory<ObjectMappingForTest>(uow);

            ObjectMappingForTest sourceObjectMappingForTest = new ObjectMappingForTest();
            sourceObjectMappingForTest.SubObject = new SubObjectMappingForTest() { };

            BaseResultType baseResultType = validateDomainObjectFactory.GetValidate(sourceObjectMappingForTest, ExecuteTypeConstCRUD.ADD);

            Assert.IsFalse(baseResultType.IsSuccess);
            Assert.AreEqual("Объект не найден в контексте для проверки обязательных полей", baseResultType.Message);

            context.Set<ObjectMappingForTest>().Add(sourceObjectMappingForTest);
            baseResultType = validateDomainObjectFactory.GetValidate(sourceObjectMappingForTest, ExecuteTypeConstCRUD.ADD);
            string messageError = "Не пройдена проверка записи \"ObjectMappingForTest\":" + Environment.NewLine +
                "Не заполнено значение \"IntValue\"." + Environment.NewLine +
                "Не заполнено значение \"Строка\".";
            Assert.IsFalse(baseResultType.IsSuccess);
            Assert.AreEqual(messageError, baseResultType.Message);

            sourceObjectMappingForTest.IntValue = 1;
            sourceObjectMappingForTest.IntValue2 = 1;

            baseResultType = validateDomainObjectFactory.GetValidate(sourceObjectMappingForTest, ExecuteTypeConstCRUD.ADD);
            Assert.IsTrue(baseResultType.IsSuccess);
            Assert.IsTrue(String.IsNullOrWhiteSpace(baseResultType.Message));

        }

        private void CreateContext(out ApplicationContextForTest context, out UnitOfWork uow)
        {
            var options = new DbContextOptionsBuilder<ApplicationContextForTest>()
                          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                         .Options;
            context = new ApplicationContextForTest(options);
            uow = new UnitOfWork(context);
            uow.SaveChanges();
        }

        [TestMethod]
        public void GetValidateTest_AddCustomValidate()
        {
            ApplicationContextForTest context;
            UnitOfWork uow;
            CreateContext(out context, out uow);

            ValidateDomainObjectFactoryForTest validateDomainObjectFactory = new ValidateDomainObjectFactoryForTest(uow);

            ObjectMappingForTest sourceObjectMappingForTest = new ObjectMappingForTest();
            sourceObjectMappingForTest.SubObject = new SubObjectMappingForTest() { };
            sourceObjectMappingForTest.IntValue = 1;
            sourceObjectMappingForTest.IntValue2 = 1;
            context.Set<ObjectMappingForTest>().Add(sourceObjectMappingForTest);
            //Проверка без заполнения StrValue. Будет ошибка, так как пустая строка
            BaseResultType baseResultType = validateDomainObjectFactory.GetValidate(sourceObjectMappingForTest, ExecuteTypeConstCRUD.ADD);

            Assert.IsFalse(baseResultType.IsSuccess);
            Assert.AreEqual("Пустое значение StrValue.", baseResultType.Message);

            //Проверка, что условие кастомной проверки не выполнилось
            sourceObjectMappingForTest.StrValue = "1";
            baseResultType = validateDomainObjectFactory.GetValidate(sourceObjectMappingForTest, ExecuteTypeConstCRUD.ADD);
            Assert.IsTrue(baseResultType.IsSuccess);
            Assert.IsTrue(String.IsNullOrWhiteSpace(baseResultType.Message));
        }

        [TestMethod]
        public void QuestionTest()
        {
            ApplicationContextForTest context;
            UnitOfWork uow;
            CreateContext(out context, out uow);

            QuestionValidateFactoryForTest validateDomainObjectFactory = new QuestionValidateFactoryForTest(uow);

            ObjectMappingForTest sourceObjectMappingForTest = new ObjectMappingForTest();
            sourceObjectMappingForTest.SubObject = new SubObjectMappingForTest() { };
            sourceObjectMappingForTest.IntValue = 1;
            sourceObjectMappingForTest.IntValue2 = 1;
            context.Set<ObjectMappingForTest>().Add(sourceObjectMappingForTest);
            //Проверка без заполнения StrValue. Будет предупреждение
            BaseResultType baseResultType = validateDomainObjectFactory.GetValidate(sourceObjectMappingForTest, ExecuteTypeConstCRUD.ADD);

            Assert.IsTrue(baseResultType.IsSuccess);
            Assert.AreEqual(1, baseResultType.Question.Count);
            Assert.AreEqual("Предупреждение", baseResultType.Question[0].Message);
        }
    }
}
