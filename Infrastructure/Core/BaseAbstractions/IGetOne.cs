using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IGetOne<T>
    {
        Task<T> GetOne(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes);
    }
}