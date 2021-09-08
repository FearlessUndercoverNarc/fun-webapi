using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Core
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> AggregateIncludes<T>(this IQueryable<T> queryable, Expression<Func<T, object>>[] includes) where T : class
        {
            return includes.Aggregate(queryable, (current, include) => current.Include(include));
        }

        public static IQueryable<T> ApplyPredicate<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate)
        {
            return predicate == null ? queryable : queryable.Where(predicate);
        }

        public static IQueryable<T> For<T, TParam>(this IQueryable<T> queryable, Expression<Func<T, TParam>> field, params Expression<Func<TParam, object>>[] includes) 
            where T : class 
            where TParam : class
        {
            var includableQueryable = queryable.Include(field);

            foreach (var include in includes)
            {
                includableQueryable.ThenInclude(include);
            }
            
            return includableQueryable;
        }
    }
}