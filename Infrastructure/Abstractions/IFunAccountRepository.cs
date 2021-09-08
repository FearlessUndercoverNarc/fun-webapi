using Infrastructure.Core.BaseAbstractions;
using Models.Db.Account;

namespace Infrastructure.Abstractions
{
    using T = FunAccount;
    public interface IFunAccountRepository  : IGetById<T>, IGetOne<T>, IAdd<T>, IUpdate<T>, IRemove<T>, ICount<T>, IGetMany<T>
    {
    }
}