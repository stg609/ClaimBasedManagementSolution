using System;
using System.Collections.Generic;

namespace Common.Infra
{
    public static class Extensions
    {
        public static List<TSource> EmptyListIfEmpty<TSource>(this IEnumerable<TSource> list)
        {
            Type genericArgument = typeof(TSource);
            Type listType = typeof(List<>);
            Type finalType = listType.MakeGenericType(genericArgument);
            
            if (list == null)
            {
                return Activator.CreateInstance(finalType) as List<TSource>;
            }
            else
            {
                return Activator.CreateInstance(finalType, list) as List<TSource>;
            }
        }
    }
}
