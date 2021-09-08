using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IGetMany<T>
    {
        Task<ICollection<T>> GetMany(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes);

        Task<ICollection<T>> GetManyReversed(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes);

        Task<ICollection<T>> GetMany(Expression<Func<T, bool>> predicate = null, int offset = 0, int limit = 25, params Expression<Func<T, object>>[] includes);

        Task<ICollection<T>> GetManyReversed(Expression<Func<T, bool>> predicate = null, int offset = 0, int limit = 25, params Expression<Func<T, object>>[] includes);


        async Task<ICollection<T>> GetMany(params Expression<Func<T, object>>[] includes)
        {
            return await GetMany(null, includes);
        }
    }
}