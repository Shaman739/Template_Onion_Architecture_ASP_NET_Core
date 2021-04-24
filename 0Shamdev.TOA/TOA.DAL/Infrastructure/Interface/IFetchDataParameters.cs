using Shamdev.ERP.DAL.Common.Interface;
using System.Collections.Generic;

namespace Shamdev.TOA.DAL.Infrastructure.Interface
{
    public interface IFetchDataParameters
    {
        int CountOnPage { get; set; }
        ICollection<IWhereDinamicItem> Filters { get; set; }
        bool IsOnlyShowData { get; set; }
        int PageNumber { get; set; }

        void CheckAndResetParam();
    }
}