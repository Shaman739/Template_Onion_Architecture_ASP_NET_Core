using Shamdev.ERP.DAL.Common.Interface;
using Shamdev.TOA.Core.Data.Consts;
using System.Collections.Generic;

namespace Shamdev.TOA.DAL.Infrastructure
{
    /// <summary>
    /// Параметры запроса на получения данных из БД
    /// </summary>
    public class FetchDataParameters
    {
        public FetchDataParameters()
        {
            PageNumber = PagingConsts.DEFAULT_PAGE_NUMBER;
            CountOnPage = PagingConsts.DEFAULT_PAGE_COUNT_ROW;
            IsOnlyShowData = false;
            Filters = new List<IWhereDinamicItem>();
        }
        public FetchDataParameters(int pageNumber, int countOnPage, bool isOnlyShowData = true):this()
        {
            PageNumber = pageNumber;
            CountOnPage = countOnPage;
            IsOnlyShowData = isOnlyShowData;
        }
        public int PageNumber { get; set; }
        public int CountOnPage { get; set; }
        public bool IsOnlyShowData { get; set; }

        public ICollection<IWhereDinamicItem> Filters { get; set; }

        /// <summary>
        /// Проверка на валидность значений поиска. Если pageNumber < 1, то PageNumber=1.Если CountOnPage < 1, то CountOnPage= PagingConsts.DEFAULT_PAGE_COUNT_ROW.
        /// </summary>
        public void CheckAndResetParam()
        {
            if (PageNumber < 1)
                PageNumber = 1;
            if (CountOnPage < 1)
                CountOnPage = PagingConsts.DEFAULT_PAGE_COUNT_ROW;
        }

    }
}
