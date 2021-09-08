using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Models.Db.Common;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IGetById<T> where T : IdEntity
    {
        Task<T> GetById(long id, params Expression<Func<T, object>>[] includes);
    }
}