using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Detech if an entity is already attached in the DBContext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <see cref="https://stackoverflow.com/questions/10027493/entityframework-code-first-check-if-entity-is-attached"/>
        /// <returns></returns>
        public static bool Exists<TContext, TEntity>(this TContext context, TEntity entity)
            where TContext : DbContext
            where TEntity : class
        {
            return context.Set<TEntity>().Local.Any(e => e == entity);
        }
    }
}
