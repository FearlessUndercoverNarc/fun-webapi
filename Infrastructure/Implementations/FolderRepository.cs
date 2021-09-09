using Infrastructure.Abstractions;
using Infrastructure.Core;
using Infrastructure.Core.BaseImplementations;
using Models.Db.Tree;

namespace Infrastructure.Implementations
{
    public class FolderRepository : IdRepositoryBase<Folder>, IFolderRepository
    {
        public FolderRepository(FunDbContext context) : base(context)
        {
        }
    }
}