using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Infrastructure.Core.BaseImplementations;
using Microsoft.EntityFrameworkCore;
using Models.Db.Tree;

namespace Infrastructure.Implementations
{
    public class FolderRepository : IdRepositoryBase<Folder>, IFolderRepository
    {
        public FolderRepository(FunDbContext context) : base(context)
        {
        }

        public async Task<bool> IsParentSharedTo(long id, long accountId)
        {
            return await GetDbSetT().AnyAsync(f => f.Parent.SharedToRelation.Any(s=>s.FunAccountId == accountId));
        }
    }
}