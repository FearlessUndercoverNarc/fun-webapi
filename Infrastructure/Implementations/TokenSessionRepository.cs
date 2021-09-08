using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Infrastructure.Core.BaseImplementations;
using Microsoft.EntityFrameworkCore;
using Models.Db.Sessions;

namespace Infrastructure.Implementations
{
    public class TokenSessionRepository : IdRepositoryBase<TokenSession>, ITokenSessionRepository
    {
        public TokenSessionRepository(FunDbContext context) : base(context)
        {
        }

        public async Task<TokenSession> GetByToken(string token)
        {
            return await Context.TokenSessions.FirstOrDefaultAsync(ts => ts.Token == token);
        }

        public async Task<ICollection<TokenSession>> GetActiveByAccount(long funId)
        {
            return await Context.TokenSessions
                .OrderBy(ts => ts.Id)
                .Where(ts => ts.FunAccountId == funId && ts.EndDate > DateTime.Now)
                .ToListAsync();
        }

        public async Task<TokenSession> GetLastByWorker(long funId)
        {
            return await Context.TokenSessions.Where(ts => ts.FunAccountId == funId).OrderBy(ts => ts.Id).LastOrDefaultAsync();
        }
    }
}