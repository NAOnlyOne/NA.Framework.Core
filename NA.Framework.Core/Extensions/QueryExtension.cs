using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace NA.Framework.Core.Extensions
{
    public static class QueryExtension
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string ordering, params object[] values)
        {
            return !string.IsNullOrWhiteSpace(ordering) ? DynamicQueryableExtensions.OrderBy(query, ordering, values) : query;
        }
    }
}
