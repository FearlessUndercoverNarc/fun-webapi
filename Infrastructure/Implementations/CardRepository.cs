using Infrastructure.Abstractions;
using Infrastructure.Core;
using Infrastructure.Core.BaseImplementations;
using Models.Db.Tree;

namespace Infrastructure.Implementations
{
    public class CardRepository : IdRepositoryBase<Card>, ICardRepository
    {
        public CardRepository(FunDbContext context) : base(context)
        {
        }
    }
}