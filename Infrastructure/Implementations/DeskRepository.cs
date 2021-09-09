using Infrastructure.Abstractions;
using Infrastructure.Core;
using Infrastructure.Core.BaseImplementations;
using Models.Db.Tree;

namespace Infrastructure.Implementations
{
    public class DeskRepository : IdRepositoryBase<Desk>, IDeskRepository
    {
        public DeskRepository(FunDbContext context) : base(context)
        {
        }
    }
}