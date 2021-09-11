using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Infrastructure.Core.BaseImplementations;
using Microsoft.EntityFrameworkCore;
using Models.Db.Relations;

namespace Infrastructure.Implementations
{
    public class DeskShareRepository : NoIdRepositoryBase<DeskShare>, IDeskShareRepository
    {
        public DeskShareRepository(FunDbContext context) : base(context)
        {
        }

        public async Task<bool> HasSharedWriteTo(long id, long recipientId)
        {
            return await GetDbSetT().FirstOrDefaultAsync(s => s.DeskId == id && s.FunAccountId == recipientId && s.HasWriteAccess) != null;
        }

        public async Task<bool> HasSharedReadTo(long id, long recipientId)
        {
            return await GetDbSetT().FirstOrDefaultAsync(s => s.DeskId == id && s.FunAccountId == recipientId) != null;
        }

        public async Task<ICollection<DeskShare>> GetShares(long id)
        {
            return await GetDbSetT()
                .Where(e => e.DeskId == id)
                .Include(e => e.FunAccount)
                .ToListAsync();
        }

        public async Task<ICollection<long>> GetIndividuallyShared(long accountId)
        {
            return await GetDbSetT()
                .Where(s =>
                    s.FunAccountId == accountId &&
                    s.Desk.Parent.SharedToRelation.All(sh => sh.FunAccountId != accountId) &&
                    !s.Desk.IsInTrashBin
                )
                .Select(s =>
                    s.DeskId
                ).ToListAsync();
        }
    }
}