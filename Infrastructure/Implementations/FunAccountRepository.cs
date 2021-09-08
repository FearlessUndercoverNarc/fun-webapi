using Infrastructure.Abstractions;
using Infrastructure.Core;
using Infrastructure.Core.BaseImplementations;
using Models.Db.Account;

namespace Infrastructure.Implementations
{
    public class FunAccountRepository : IdRepositoryBase<FunAccount>, IFunAccountRepository
    {
        public FunAccountRepository(FunDbContext context) : base(context)
        {
        }
    }
}