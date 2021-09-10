using Infrastructure.Core.BaseAbstractions;
using Models.Db.Tree;

namespace Infrastructure.Abstractions
{
    using T = CardConnection;

    public interface ICardConnectionRepository : IAdd<T>, IRemove<T>, IGetMany<T>, IGetOne<T>, IGetById<T>
    {
    }
}