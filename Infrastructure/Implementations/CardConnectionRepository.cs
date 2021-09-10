using Infrastructure.Abstractions;
using Infrastructure.Core;
using Infrastructure.Core.BaseImplementations;
using Models.Db.Tree;

namespace Infrastructure.Implementations
{
    public class CardConnectionRepository : IdRepositoryBase<CardConnection>, ICardConnectionRepository
    {
        public CardConnectionRepository(FunDbContext context) : base(context)
        {
        }
    }
}