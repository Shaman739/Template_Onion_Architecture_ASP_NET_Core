using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shamdev.TOA.Core.Data.Consts;
using Shamdev.TOA.DAL.Infrastructure;

namespace UnitTestProject.DAL.Infrastructure
{
    [TestClass]
    public class FetchDataParametersTest
    {
        [TestMethod]
        public void ConstructorDefaultValuesTest()
        {
            FetchDataParameters fetchDataParameters = new FetchDataParameters();
            Assert.AreEqual(1, fetchDataParameters.PageNumber);
            Assert.AreEqual(PagingConsts.DEFAULT_PAGE_COUNT_ROW, fetchDataParameters.CountOnPage);
            Assert.IsFalse(fetchDataParameters.IsOnlyShowData);

            fetchDataParameters = new FetchDataParameters(20, 30, true);
            Assert.AreEqual(20, fetchDataParameters.PageNumber);
            Assert.AreEqual(30, fetchDataParameters.CountOnPage);
            Assert.IsTrue(fetchDataParameters.IsOnlyShowData);
            Assert.IsNotNull(fetchDataParameters.Filters);
            Assert.AreEqual(0,fetchDataParameters.Filters.Count);

        }

        [TestMethod]
        public void CheckAndResetParamTest()
        {
            FetchDataParameters fetchDataParameters = new FetchDataParameters();
            fetchDataParameters.PageNumber = 0;
            fetchDataParameters.CountOnPage = 0;
            fetchDataParameters.CheckAndResetParam();
            Assert.AreEqual(1, fetchDataParameters.PageNumber);
            Assert.AreEqual(PagingConsts.DEFAULT_PAGE_COUNT_ROW, fetchDataParameters.CountOnPage);

            fetchDataParameters.PageNumber = 10;
            fetchDataParameters.CountOnPage = 20;
            Assert.AreEqual(10, fetchDataParameters.PageNumber);
            Assert.AreEqual(20, fetchDataParameters.CountOnPage);
            Assert.IsNotNull(fetchDataParameters.Filters);
            Assert.AreEqual(0, fetchDataParameters.Filters.Count);
        }
    }
}
