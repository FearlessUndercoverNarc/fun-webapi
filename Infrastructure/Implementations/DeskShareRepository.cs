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

        public async Task<bool> IsSharedTo(long id, long recipientId)
        {
            return await GetDbSetT().FirstOrDefaultAsync(s => s.DeskId == id && s.FunAccountId == recipientId) != null;
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