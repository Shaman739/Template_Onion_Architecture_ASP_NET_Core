using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.DAL.ValidateContext;
using System;

namespace UnitTestProject.DAL.ValidateContext
{
    [TestClass]
    public class ValidateContextResultTest
    {
        [TestMethod]
        public void AddEntityTest()
        {
            ValidateContextResult validateContextResult = new ValidateContextResult();
            validateContextResult.IsSuccess = true;
            Assert.IsTrue(String.IsNullOrWhiteSpace(validateContextResult.Message), "По умолчанию сообщение пусто.");

            //Добавим одну невалидную сущность
            ValidateContextResultItem validateContextResultItem = new ValidateContextResultItem();
            validateContextResultItem.Name = "Объект1";
            validateContextResultItem.Fields.Add(new ValidateContextResultItem() { Name = "Поле1" });
            validateContextResultItem.Fields.Add(new ValidateContextResultItem() { Name = "Поле2" });
            validateContextResult.AddEntity(validateContextResultItem);

            string messageError = "Не пройдена проверка записи \"Объект1\":" + Environment.NewLine +
                "Не заполнено значение \"Поле1\"." + Environment.NewLine +
                "Не заполнено значение \"Поле2\".";
            Assert.IsFalse(validateContextResult.IsSuccess);
            Assert.AreEqual(messageError, validateContextResult.Message);

            ValidateContextResultItem validateContextResultItem2 = new ValidateContextResultItem();
            validateContextResultItem2.Name = "Объект2";
            validateContextResultItem2.Fields.Add(new ValidateContextResultItem() { Name = "Поле3" });
            validateContextResultItem2.Fields.Add(new ValidateContextResultItem() { Name = "Поле4" });
            validateContextResult.AddEntity(validateContextResultItem2);

            messageError = "Не пройдена проверка записи \"Объект1\":" + Environment.NewLine +
               "Не заполнено значение \"Поле1\"." + Environment.NewLine +
               "Не заполнено значение \"Поле2\"." + Environment.NewLine +
               Environment.NewLine +
            "Не пройдена проверка записи \"Объект2\":" + Environment.NewLine +
               "Не заполнено значение \"Поле3\"." + Environment.NewLine +
               "Не заполнено значение \"Поле4\".";
            Assert.IsFalse(validateContextResult.IsSuccess);
            Assert.AreEqual(messageError, validateContextResult.Message);

        }
    }
}
