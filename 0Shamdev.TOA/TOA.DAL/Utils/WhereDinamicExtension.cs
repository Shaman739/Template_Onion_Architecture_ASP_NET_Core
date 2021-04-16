using Shamdev.ERP.DAL.Common.Interface;
using Shamdev.ERP.DAL.Common.Utils;
using Shamdev.TOA.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    static class WhereDinamicExtension
    {
        public static IQueryable<TEntity> WheryDinamic<TEntity>(this IQueryable<TEntity> source, ICollection<IWhereDinamicItem> filters) where TEntity : DomainObject, new()
        {
            foreach (var filter in filters)
                source = source.Where(ExpressionUtils.BuildPredicate<TEntity>(filter.PropertyName, filter.TypeFilter, filter.Value));

            return source;
        }
    }
}
