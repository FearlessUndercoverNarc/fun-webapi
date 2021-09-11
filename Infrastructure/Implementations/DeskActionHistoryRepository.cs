using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Infrastructure.Core.BaseImplementations;
using Microsoft.EntityFrameworkCore;
using Models.Db.Tree;

namespace Infrastructure.Implementations
{
    public class DeskActionHistoryRepository : VersionedRepositoryBase<DeskActionHistoryItem>, IDeskActionHistoryRepository
    {
        public DeskActionHistoryRepository(FunDbContext context) : base(context)
        {
        }

        public async Task<uint> GetLastVersionByDesk(long deskId)
        {
            return await GetDbSetT().Where(e => e.DeskId == deskId).MaxAsync(e => e.Version);
        }
    }
}