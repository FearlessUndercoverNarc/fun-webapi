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

        public async Task<bool> IsSharedTo(long id, long recipientId)
        {
            return await GetDbSetT().FirstOrDefaultAsync(s => s.FolderId == id && s.FunAccountId == recipientId) != null;
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