using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.ERP.DAL.Common.Interface
{
    /// <summary>
    /// Параметр для динамической фильтрации
    /// </summary>
    public interface IWhereDinamicItem

    {
        string PropertyName { get; set; }
        TypeFilterEnum TypeFilter { get; set; }

        string Value { get; set; }
    }

    public enum TypeFilterEnum
    {
        EQUAL,
        NOT_EQUAL,
        GREATER_THAN,
        GREATER_THAN_OR_EQUAL,
        LESS_THAN,
        LESS_THAN_OR_EQUAL,
        CONTAINS,
        STARTS_WITH,
        ENDS_WITH
    }

}
