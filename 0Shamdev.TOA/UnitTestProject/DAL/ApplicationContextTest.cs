using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UnitTestProject.DAL.TestFakeClasses;

namespace UnitTestProject.DAL
{
    [TestClass]
    public class ApplicationContextTest
    {
        [TestMethod]
        [Ignore]
        public void ValidateContextTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationContextForTest>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;
            ApplicationContextForTest context = new ApplicationContextForTest(options);
            ObjectMappingForTest objectMappingForTest = new ObjectMappingForTest();
            context.SaveChanges();
            context.Set<ObjectMappingForTest>().Add(objectMappingForTest);
           // var result = Assert.ThrowsException<DbEntityValidationException>(() => context.SaveChanges());

            //Assert.IsFalse(validateContextResult.IsSuccess);

            //Assert.AreEqual(1, validateContextResult.Count);
            //Assert.AreEqual(2, validateContextResult[0].Fields.Count);
            //Assert.AreEqual("IntValue", validateContextResult[0].Fields[0].Name);
            //Assert.AreEqual("Строка", validateContextResult[0].Fields[1].Name);
        }
    }
}
