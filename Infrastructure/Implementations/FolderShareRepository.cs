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
    public class FolderShareRepository : NoIdRepositoryBase<FolderShare>, IFolderShareRepository
    {
        public FolderShareRepository(FunDbContext context) : base(context)
        {
        }

        public async Task<bool> HasSharedWriteTo(long id, long recipientId)
        {
            return await GetDbSetT().FirstOrDefaultAsync(s => s.FolderId == id && s.FunAccountId == recipientId && s.HasWriteAccess) != null;
        }

        public async Task<bool> HasSharedReadTo(long id, long recipientId)
        {
            return await GetDbSetT().FirstOrDefaultAsync(s => s.FolderId == id && s.FunAccountId == recipientId) != null;
        }
        
        public async Task<ICollection<FolderShare>> GetShares(long id)
        {
            return await GetDbSetT()
                .Where(e => e.FolderId == id)
                .Include(e => e.FunAccount)
                .ToListAsync();
        }

        public async Task<ICollection<long>> GetSharedRoots(long accountId)
        {
            return await GetDbSetT()
                .Where(s =>
                    s.FunAccountId == accountId &&
                    s.Folder.Parent.SharedToRelation.All(sh => sh.FunAccountId != accountId) &&
                    !s.Folder.IsInTrashBin
                )
                .Select(s =>
                    s.FolderId
                ).ToListAsync();
        }
    }
}