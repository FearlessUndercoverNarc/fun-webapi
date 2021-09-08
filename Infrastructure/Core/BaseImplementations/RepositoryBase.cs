using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Core.BaseImplementations
{
    public abstract class RepositoryBase<T> where T : class
    {
        protected readonly FunDbContext Context;

        protected RepositoryBase(FunDbContext context)
        {
            Context = context;
        }

        protected DbSet<T> GetDbSetT() => Context.Set<T>();

        public async Task<long> Count(Expression<Func<T, bool>> predicate = null)
        {
            return await GetDbSetT().ApplyPredicate(predicate).LongCountAsync();
        }

        public async Task<T> GetOne(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes)
        {
            return await GetDbSetT().AggregateIncludes(includes).ApplyPredicate(predicate).FirstOrDefaultAsync();
        }
    }
}