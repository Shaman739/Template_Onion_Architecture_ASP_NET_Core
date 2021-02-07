using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.ERP.Core.Data.Infrastructure.ResultType.Question;
using Shamdev.TOA.BLL;
using Shamdev.TOA.BLL.Infrastructure;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Infrastructure.PrepareItemForCRUDOperations.Interface;
using Shamdev.TOA.BLL.Infrastructure.ResultType;
using Shamdev.TOA.BLL.Interface;
using Shamdev.TOA.BLL.Validate.Interface;
using Shamdev.TOA.Core.Data;
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
            public ObjectMappingForTest GetItem(DefaultParamOfCRUDOperation<ObjectMappingForTest> item)
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

        class ValidateWithWarrningDomainObject : IValidateDomainObject<ObjectMappingForTest>
        {
            public BaseResultType Validate(DefaultParamOfCRUDOperation<ObjectMappingForTest> item)
            {
                BaseResultType baseResultType = new BaseResultType() { Status = ResultStatus.Success };
                if (String.IsNullOrWhiteSpace(item.Item.StrValue))
                {
                    WarningQuestion question = new WarningQuestion()
                    {
                        Id = "1",
                        Message = "Отсутствует строка"
                    };
                    baseResultType.AddWarring(question);
                }

                return baseResultType;
            }
        }


        class SaveWithWarrningDefaultCRUDBLLForTest : DefaultCRUDBLL<ObjectMappingForTest>
        {
            public SaveWithWarrningDefaultCRUDBLLForTest(IUnitOfWork contextDB) : base(contextDB)
            {
                ValidateDomainObject.Value.AddStrategy(ValidateTypeConstCRUD.ADD_OR_EDIT, new Lazy<IValidateDomainObject<ObjectMappingForTest>>(() => new ValidateWithWarrningDomainObject()));
            }
        }

        class ValidateWithQuestionDomainObject : IValidateDomainObject<ObjectMappingForTest>
        {
            public BaseResultType Validate(DefaultParamOfCRUDOperation<ObjectMappingForTest> item)
            {
                BaseResultType baseResultType = new BaseResultType() { Status = ResultStatus.Success };

                if (String.IsNullOrWhiteSpace(item.Item.StrValue))
                {
                    if (!item.IsSendAndAnswerQuestion("Продолжить?", ResultQuestionType.NO, baseResultType))
                        baseResultType.AddError("Ошибка");
                }

                if (item.Item.IntValue2 == 22)
                {
                    baseResultType.AddWarring(new WarningQuestion() { Message = "Уведомления" });
                }
                return baseResultType;
            }
        }
        class SaveWithQuestionDefaultCRUDBLLForTest : DefaultCRUDBLL<ObjectMappingForTest>
        {
            public SaveWithQuestionDefaultCRUDBLLForTest(IUnitOfWork contextDB) : base(contextDB)
            {
                ValidateDomainObject.Value.AddStrategy(ValidateTypeConstCRUD.ADD_OR_EDIT, new Lazy<IValidateDomainObject<ObjectMappingForTest>>(() => new ValidateWithQuestionDomainObject()));
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
            _uow.SaveChangesAsync();
        }

        private IFetchData<TEntity> GetFetchData<TEntity>() where TEntity : DomainObject,new()
        {
            return new FetchDomainData<TEntity>(_uow);
        }
        [TestMethod]
        public void SaveItemTest()
        {
            DefaultCRUDBLL<ObjectMappingForTest> bll = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, null).Result;
            Assert.AreEqual(ResultStatus.Fail, resultCRUDOpeartion.Status);
            StringAssert.Contains(resultCRUDOpeartion.Message, "Объект для добавления/изменения не может быть null.");

            resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, new DefaultParamOfCRUDOperation<ObjectMappingForTest>()).Result;
            Assert.AreEqual(ResultStatus.Fail, resultCRUDOpeartion.Status);
            StringAssert.Contains(resultCRUDOpeartion.Message, "Объект для добавления/изменения не может быть null.");

            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { IntValue = 11, StrValue = "22" };

            Assert.AreEqual(1, GetFetchData<ObjectMappingForTest>().FetchDataAsync(new FetchDataParameters()).Result.TotalCountRows);
            resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;

            Assert.AreEqual(ResultStatus.Fail, resultCRUDOpeartion.Status, "У объекта не заполнено обязательное поле IntValue2. Такое не должно проходить валидацию по контексту");

            objectFroCRUD.Item.IntValue2 = 33;
            //Пересоздаем контекст, так как были в него добавления других объектов, которые не сохранились.
            CreateContext();
            bll = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;
            Assert.AreEqual(ResultStatus.Success, resultCRUDOpeartion.Status);
            Assert.IsNotNull(resultCRUDOpeartion.Data, "После сохранения должен отдаваться сохраненный объект.");
            Assert.IsInstanceOfType(resultCRUDOpeartion.Data, typeof(SaveResultType<ObjectMappingForTest>));
            Assert.AreEqual(2, resultCRUDOpeartion.Data.Item.Id);
            Assert.AreEqual(11, ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).IntValue);
            Assert.AreEqual("22", ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).StrValue);
            ResultFetchData<ObjectMappingForTest> allDataInDB = GetFetchData<ObjectMappingForTest>().FetchDataAsync(new FetchDataParameters()).Result;
            Assert.AreEqual(2, allDataInDB.TotalCountRows, "Должна была добавиться одна запись в БД");
            Assert.AreEqual(2, allDataInDB.Items[1].Id);
            Assert.AreEqual(11, allDataInDB.Items[1].IntValue);
            Assert.AreEqual("22", allDataInDB.Items[1].StrValue);

            //Проверка на изменение
            objectFroCRUD.Item = new ObjectMappingForTest() { Id = 1, IntValue = 111, StrValue = "222", IntValue2 = 222 };
            resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.EDIT, objectFroCRUD).Result;
            Assert.AreEqual(ResultStatus.Success, resultCRUDOpeartion.Status);
            Assert.IsNotNull(resultCRUDOpeartion.Data, "После сохранения должен отдаваться сохраненный объект.");
            Assert.IsInstanceOfType(resultCRUDOpeartion.Data, typeof(SaveResultType<ObjectMappingForTest>));
            Assert.AreEqual(1, resultCRUDOpeartion.Data.Item.Id);
            Assert.AreEqual(111, ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).IntValue);
            Assert.AreEqual("222", ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).StrValue);
            allDataInDB = GetFetchData<ObjectMappingForTest>().FetchDataAsync(new FetchDataParameters()).Result;
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
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.DELETE, objectFroCRUD).Result;
            Assert.AreEqual(ResultStatus.Fail, resultCRUDOpeartion.Status);
            Assert.IsNotNull(resultCRUDOpeartion.Data, "Результат не может быть null.");
            Assert.IsNull(resultCRUDOpeartion.Data.Item, "Объекта с таким ID нет в БД.");
            Assert.AreEqual(1, GetFetchData<ObjectMappingForTest>().FetchDataAsync(new FetchDataParameters()).Result.TotalCountRows, "Не должно измениться количество строк в БД");

            //Удаление объекта
            objectFroCRUD.Item = new ObjectMappingForTest() { Id = 1 };
            resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.DELETE, objectFroCRUD).Result;
            Assert.AreEqual(ResultStatus.Success, resultCRUDOpeartion.Status);
            Assert.IsNotNull(resultCRUDOpeartion.Data, "После сохранения должен отдаваться сохраненный объект.");
            Assert.IsInstanceOfType(resultCRUDOpeartion.Data, typeof(SaveResultType<ObjectMappingForTest>));
            Assert.AreEqual(0, GetFetchData<ObjectMappingForTest>().FetchDataAsync(new FetchDataParameters()).Result.TotalCountRows, "Не должно измениться количество строк в БД");
        }
        [TestMethod]
        public void SaveItemWithErrorTest()
        {
            // В этом БЛЛ будет ошибка, так как стратегия на добавления заменена на fake с ошибкой. Нужно проверить, что результат подготовки будет с ошибкой и не будет сохранений
            DefaultCRUDBLLForTest bll = new DefaultCRUDBLLForTest(_uow);
            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { StrValue = "222222" };
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;
            Assert.AreEqual(ResultStatus.Fail, resultCRUDOpeartion.Status, "Не прошла валидация по обязательным полям контекста.");
            StringAssert.Contains("Для проверки ошибки подготовки в БЛЛ", resultCRUDOpeartion.Message, "Должно пробрасываться сообщение об ошибке из стратегии.");
            ResultFetchData<ObjectMappingForTest> allDataInDB = GetFetchData<ObjectMappingForTest>().FetchDataAsync(new FetchDataParameters()).Result;
            Assert.IsNull(allDataInDB.Items.FirstOrDefault(x => x.StrValue == "222222"), "Не должно быть записи в БД, так как ошибка подготовки была");
        }

        [TestMethod]
        public void ValidateTest()
        {
            DefaultCRUDBLL<ObjectMappingForTest> bll = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { StrValue = "22" };
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;
            Assert.AreEqual(ResultStatus.Fail, resultCRUDOpeartion.Status, "Не прошла валидация по обязательным полям контекста.");
        }

       

        [TestMethod]
        public void SaveItem_IsOnlyAddInContext_Test()
        {
            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { IntValue = 11, StrValue = "22" };
            objectFroCRUD.Item.IntValue2 = 33;



            CreateContext();
            DefaultCRUDBLL<ObjectMappingForTest> bll = new DefaultCRUDBLL<ObjectMappingForTest>(_uow);
            bll.IsOnlyAddInContext = true;
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;
            Assert.AreEqual(ResultStatus.Success, resultCRUDOpeartion.Status);
            Assert.IsNotNull(resultCRUDOpeartion.Data, "После сохранения должен отдаваться сохраненный объект.");
            Assert.IsInstanceOfType(resultCRUDOpeartion.Data, typeof(SaveResultType<ObjectMappingForTest>));
            //Вернет объект, но не сохранит его в БД
            Assert.AreEqual(2, resultCRUDOpeartion.Data.Item.Id);
            Assert.AreEqual(11, ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).IntValue);
            Assert.AreEqual("22", ((ObjectMappingForTest)resultCRUDOpeartion.Data.Item).StrValue);
            ResultFetchData<ObjectMappingForTest> allDataInDB = GetFetchData<ObjectMappingForTest>().FetchDataAsync(new FetchDataParameters()).Result;
            Assert.AreEqual(1, allDataInDB.TotalCountRows, "Должна была добавиться одна запись в БД");
            Assert.AreEqual(1, allDataInDB.Items[0].Id);
            Assert.AreEqual(2, allDataInDB.Items[0].IntValue);
            Assert.AreEqual("2", allDataInDB.Items[0].StrValue);

        }

        [TestMethod]
        public void SaveWithQuestionTest()
        {

            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { IntValue = 11 };
            objectFroCRUD.Item.IntValue2 = 33;
            CreateContext();
            SaveWithWarrningDefaultCRUDBLLForTest bll = new SaveWithWarrningDefaultCRUDBLLForTest(_uow);

            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;

            Assert.AreEqual(ResultStatus.Success, resultCRUDOpeartion.Status);
            Assert.IsNotNull(resultCRUDOpeartion.Question);
            Assert.AreEqual(1, resultCRUDOpeartion.Question.Count);
            Assert.AreEqual("Отсутствует строка", resultCRUDOpeartion.Question[0].Message);
            Assert.AreEqual(1, resultCRUDOpeartion.Question[0].Buttons.Count);
            Assert.AreEqual("ОК", resultCRUDOpeartion.Question[0].Buttons[0].Label);
        }

        [TestMethod]
        public void ValidateWithQuestionTest()
        {
            //Проверка валидации с вопрос пользователю, после которого пользователь отправит этот же объект ответ
            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { IntValue = 11 };
            objectFroCRUD.Item.IntValue2 = 33;
            objectFroCRUD.Item.CustomIdentity = "1";
            CreateContext();
            SaveWithQuestionDefaultCRUDBLLForTest bll = new SaveWithQuestionDefaultCRUDBLLForTest(_uow);

            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;

            Assert.AreEqual(ResultStatus.Question, resultCRUDOpeartion.Status);
            Assert.IsNotNull(resultCRUDOpeartion.Question);
            Assert.AreEqual(1, resultCRUDOpeartion.Question.Count);
            Assert.AreEqual("Продолжить?", resultCRUDOpeartion.Question[0].Message);
            Assert.IsFalse(String.IsNullOrWhiteSpace(resultCRUDOpeartion.Question[0].Id));
            Assert.AreEqual(2, resultCRUDOpeartion.Question[0].Buttons.Count);

            Assert.AreEqual("Да", resultCRUDOpeartion.Question[0].Buttons[0].Label);
            Assert.AreEqual("Нет", resultCRUDOpeartion.Question[0].Buttons[1].Label);
            Assert.IsInstanceOfType(resultCRUDOpeartion.Question[0].Buttons[0], typeof(ButtonQuestionWithResult));
            Assert.IsInstanceOfType(resultCRUDOpeartion.Question[0].Buttons[1], typeof(ButtonQuestionWithResult));

            ResultFetchData<ObjectMappingForTest> allDataInDB = GetFetchData<ObjectMappingForTest>().FetchDataAsync(new FetchDataParameters()).Result;
            Assert.AreEqual(1, allDataInDB.TotalCountRows, "Добавление не произошло, так как есть вопрос к пользователю");
            Assert.AreEqual(1, allDataInDB.Items[0].Id);
            Assert.AreEqual(2, allDataInDB.Items[0].IntValue);
            Assert.AreEqual("2", allDataInDB.Items[0].StrValue);

            Assert.IsInstanceOfType(resultCRUDOpeartion.Question[0], typeof(QuestionYesNo));
            //Отвечаем "Да" на вопрос из валидации
            (resultCRUDOpeartion.Question[0] as QuestionYesNo).Result = (resultCRUDOpeartion.Question[0].Buttons[0] as ButtonQuestionWithResult).Result;
            objectFroCRUD.Questions = resultCRUDOpeartion.Question;
            CreateContext();
            bll = new SaveWithQuestionDefaultCRUDBLLForTest(_uow);
            resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;
            Assert.AreEqual(ResultStatus.Success, resultCRUDOpeartion.Status);
            Assert.IsTrue(resultCRUDOpeartion.Question == null || resultCRUDOpeartion.Question.Count() == 0);

            //После ответа ДА сохранилось и должно быть 2 записи
            allDataInDB = GetFetchData<ObjectMappingForTest>().FetchDataAsync(new FetchDataParameters()).Result;
            Assert.AreEqual(2, allDataInDB.TotalCountRows, "После ответа ДА сохранилось и должно быть 2 записи в БД");
            Assert.AreEqual(1, allDataInDB.Items[0].Id);
            Assert.AreEqual(2, allDataInDB.Items[0].IntValue);
            Assert.AreEqual("2", allDataInDB.Items[0].StrValue);
            Assert.AreEqual(2, allDataInDB.Items[1].Id);
            Assert.AreEqual(33, allDataInDB.Items[1].IntValue2);
            Assert.IsTrue(String.IsNullOrWhiteSpace(allDataInDB.Items[1].StrValue));
        }

        [TestMethod]
        public void ValidateWithQuestionNotCorrectAnswerTest()
        {
            //Проверка валидации с вопрос пользователю, после которого пользователь отправит этот же объект ответ
            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { IntValue = 11 };
            objectFroCRUD.Item.IntValue2 = 33;
            objectFroCRUD.Item.CustomIdentity = "1";
            CreateContext();
            SaveWithQuestionDefaultCRUDBLLForTest bll = new SaveWithQuestionDefaultCRUDBLLForTest(_uow);

            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;

            Assert.AreEqual(ResultStatus.Question, resultCRUDOpeartion.Status);
            Assert.IsNotNull(resultCRUDOpeartion.Question);
            Assert.AreEqual(1, resultCRUDOpeartion.Question.Count);
            Assert.AreEqual("Продолжить?", resultCRUDOpeartion.Question[0].Message);
            Assert.IsFalse(String.IsNullOrWhiteSpace(resultCRUDOpeartion.Question[0].Id));
            Assert.AreEqual(2, resultCRUDOpeartion.Question[0].Buttons.Count);

            Assert.AreEqual("Да", resultCRUDOpeartion.Question[0].Buttons[0].Label);
            Assert.AreEqual("Нет", resultCRUDOpeartion.Question[0].Buttons[1].Label);
            Assert.IsInstanceOfType(resultCRUDOpeartion.Question[0].Buttons[0], typeof(ButtonQuestionWithResult));
            Assert.IsInstanceOfType(resultCRUDOpeartion.Question[0].Buttons[1], typeof(ButtonQuestionWithResult));

            ResultFetchData<ObjectMappingForTest> allDataInDB = GetFetchData<ObjectMappingForTest>().FetchDataAsync(new FetchDataParameters()).Result;
            Assert.AreEqual(1, allDataInDB.TotalCountRows, "Добавление не произошло, так как есть вопрос к пользователю");
            Assert.AreEqual(1, allDataInDB.Items[0].Id);
            Assert.AreEqual(2, allDataInDB.Items[0].IntValue);
            Assert.AreEqual("2", allDataInDB.Items[0].StrValue);

            Assert.IsInstanceOfType(resultCRUDOpeartion.Question[0], typeof(QuestionYesNo));
            //Отвечаем "Да" на вопрос из валидации
            (resultCRUDOpeartion.Question[0] as QuestionYesNo).Result = (resultCRUDOpeartion.Question[0].Buttons[0] as ButtonQuestionWithResult).Result;
            (resultCRUDOpeartion.Question[0] as QuestionYesNo).Id = "Меняем идентификатор";
            objectFroCRUD.Questions = resultCRUDOpeartion.Question;
            CreateContext();
            bll = new SaveWithQuestionDefaultCRUDBLLForTest(_uow);
            resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;
            //Будет опять тот же вопрос, так как в ответе изменен идентификатор ответа
            Assert.AreEqual(ResultStatus.Question, resultCRUDOpeartion.Status);
            Assert.IsNotNull(resultCRUDOpeartion.Question);
            Assert.AreEqual(1, resultCRUDOpeartion.Question.Count);
            Assert.AreEqual("Продолжить?", resultCRUDOpeartion.Question[0].Message);
            Assert.IsFalse(String.IsNullOrWhiteSpace(resultCRUDOpeartion.Question[0].Id));
            Assert.AreEqual(2, resultCRUDOpeartion.Question[0].Buttons.Count);

            //После ответа ДА сохранилось и должно быть 2 записи
            allDataInDB = GetFetchData<ObjectMappingForTest>().FetchDataAsync(new FetchDataParameters()).Result;
            Assert.AreEqual(1, allDataInDB.TotalCountRows, "После ответа ДА сохранилось и должно быть 2 записи в БД");
            Assert.AreEqual(1, allDataInDB.Items[0].Id);
            Assert.AreEqual(2, allDataInDB.Items[0].IntValue);
            Assert.AreEqual("2", allDataInDB.Items[0].StrValue);

        }
        [TestMethod]
        public void DeleteWarrningIfQuestionExistTest()
        {
            //При добавление вопросов нуно удалять предупреждения, так как они нужны после сохранения
            //Проверка валидации с вопрос пользователю, после которого пользователь отправит этот же объект ответ
            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { IntValue = 11 };
            objectFroCRUD.Item.IntValue2 = 22;
            objectFroCRUD.Item.CustomIdentity = "1";
            CreateContext();
            SaveWithQuestionDefaultCRUDBLLForTest bll = new SaveWithQuestionDefaultCRUDBLLForTest(_uow);

            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;

            //Будет только вопрос
            Assert.AreEqual(ResultStatus.Question, resultCRUDOpeartion.Status);
            Assert.IsNotNull(resultCRUDOpeartion.Question);
            Assert.AreEqual(1, resultCRUDOpeartion.Question.Count);
            Assert.AreEqual("Продолжить?", resultCRUDOpeartion.Question[0].Message);
            Assert.IsFalse(String.IsNullOrWhiteSpace(resultCRUDOpeartion.Question[0].Id));
            Assert.AreEqual(2, resultCRUDOpeartion.Question[0].Buttons.Count);
            Assert.IsInstanceOfType(resultCRUDOpeartion.Question[0], typeof(QuestionYesNo));
            //Добавим ответы, чтобы появилось уведомление
            (resultCRUDOpeartion.Question[0] as QuestionYesNo).Result = (resultCRUDOpeartion.Question[0].Buttons[0] as ButtonQuestionWithResult).Result;
            objectFroCRUD.Questions = resultCRUDOpeartion.Question;
            CreateContext();
            bll = new SaveWithQuestionDefaultCRUDBLLForTest(_uow);
            resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;
            Assert.AreEqual(ResultStatus.Success, resultCRUDOpeartion.Status);
            Assert.IsNotNull(resultCRUDOpeartion.Question);
            Assert.AreEqual(1, resultCRUDOpeartion.Question.Count);
            Assert.AreEqual("Уведомления", resultCRUDOpeartion.Question[0].Message);
            Assert.IsInstanceOfType(resultCRUDOpeartion.Question[0], typeof(WarningQuestion));

        }

        [TestMethod]
        public void ExceptionforItemWithoutCustomIdInObject()
        {
            DefaultParamOfCRUDOperation<ObjectMappingForTest> objectFroCRUD = new DefaultParamOfCRUDOperation<ObjectMappingForTest>();
            objectFroCRUD.Item = new ObjectMappingForTest() { IntValue = 11 };
            objectFroCRUD.Item.IntValue2 = 22;
            CreateContext();
            SaveWithQuestionDefaultCRUDBLLForTest bll = new SaveWithQuestionDefaultCRUDBLLForTest(_uow);
            BaseResultType<SaveResultType<ObjectMappingForTest>> resultCRUDOpeartion = bll.SaveItemAsync(ExecuteTypeConstCRUD.ADD, objectFroCRUD).Result;

            Assert.AreEqual(ResultStatus.Fail, resultCRUDOpeartion.Status);
            Assert.AreEqual("Отсутствует идентификатор объекта для вопроса.", resultCRUDOpeartion.Message);
        }
    }
}
