using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Core.BaseImplementations
{
    public class NoIdRepositoryBase<T> : RepositoryBase<T> where T : class
    {
        protected NoIdRepositoryBase(FunDbContext context) : base(context)
        {
        }

        public async Task Update(T entity)
        {
            GetDbSetT().Update(entity);
            await Context.SaveChangesAsync();
        }

        public async Task Remove(T entity)
        {
            GetDbSetT().Remove(entity);
            await Context.SaveChangesAsync();
        }

        public async Task RemoveMany(ICollection<T> entities)
        {
            GetDbSetT().RemoveRange(entities);
            await Context.SaveChangesAsync();
        }

        public async Task AddMany(ICollection<T> entities)
        {
            GetDbSetT().AddRange(entities);
            await Context.SaveChangesAsync();
        }

        public async Task Add(T entity)
        {
            GetDbSetT().Add(entity);
            await Context.SaveChangesAsync();
        }

        public async Task<ICollection<T>> GetMany(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes)
        {
            return await GetDbSetT().AggregateIncludes(includes).ApplyPredicate(predicate).ToListAsync();
        }
    }
}