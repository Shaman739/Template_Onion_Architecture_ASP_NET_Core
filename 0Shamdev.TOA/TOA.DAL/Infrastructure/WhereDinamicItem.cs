using Shamdev.ERP.DAL.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamdev.ERP.DAL.Common.Infrastructure
{
    public class WhereDinamicItem : IWhereDinamicItem
    {
        public WhereDinamicItem(string propertyName, TypeFilterEnum typeFilter,string value)
        {
            PropertyName = propertyName;
            TypeFilter = typeFilter;
            Value = value;
        }
        public string PropertyName { get; set; }
        public TypeFilterEnum TypeFilter { get; set; }
        public string Value { get; set; }
    }
}
