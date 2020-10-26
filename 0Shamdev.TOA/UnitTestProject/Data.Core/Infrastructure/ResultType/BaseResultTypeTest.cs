using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System;

namespace UnitTestProject.Data.Core.Infrastructure.ResultType
{
    [TestClass]
    public class BaseResultTypeTest
    {
        [TestMethod]
        public void AddMessageTest()
        {
            BaseResultType baseResultType = new BaseResultType();

            baseResultType.IsSuccess = false;
            baseResultType.AddMessage("Message1");
            Assert.AreEqual("Message1", baseResultType.Message);
            Assert.IsFalse(baseResultType.IsSuccess, "При добавлении сообщения,IsSuccess не должен меняться. Добавялется только сообщение.");

            baseResultType.IsSuccess = true;
            baseResultType.AddMessage("Message2");
            Assert.AreEqual("Message1" + Environment.NewLine + "Message2", baseResultType.Message);
            Assert.IsTrue(baseResultType.IsSuccess, "При добавлении сообщения,IsSuccess не должен меняться. Добавялется только сообщение.");
        }
        [TestMethod]
        public void AddErrorTest()
        {
            BaseResultType baseResultType = new BaseResultType();

            baseResultType.IsSuccess = false;
            baseResultType.AddError("Error1");
            Assert.AreEqual("Error1", baseResultType.Message);
            Assert.IsFalse(baseResultType.IsSuccess, "При добавлении ошибки,IsSuccess  должен меняться на false.");

            baseResultType.IsSuccess = true;
            baseResultType.AddError("Error2");
            Assert.AreEqual("Error1" + Environment.NewLine + "Error2", baseResultType.Message);
            Assert.IsFalse(baseResultType.IsSuccess, "При добавлении ошибки,IsSuccess  должен меняться на false.");
        }

        [TestMethod]
        public void MergeTest()
        {
            BaseResultType error = new BaseResultType();
            error.IsSuccess = false;
            error.AddError("Error");

            BaseResultType message = new BaseResultType();
            message.IsSuccess = true;
            message.AddMessage("Message");

            //При merge, IsSuccess не должен изменяться из false в true, но должен меняться из true в false. Если была ошибка, то ошибка должна остаться 
            error.Merge(message);
            Assert.IsFalse(error.IsSuccess);
            Assert.AreEqual("Error" + Environment.NewLine + "Message", error.Message);

            error.Message = "Error";//Сьрасываем сообщение на исходное
            message.Merge(error);
            Assert.IsFalse(error.IsSuccess);
            Assert.AreEqual("Message" + Environment.NewLine + "Error", message.Message);


        }

    }
}
