using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IGetLast<T>
    {
        Task<T> GetLast(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes);
    }
}