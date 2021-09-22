using Infrastructure.Core.BaseAbstractions;
using Models.Db.Tree;

namespace Infrastructure.Abstractions
{
    using T = Card;

    public interface ICardRepository : IAdd<T>, IAddMany<T>, IUpdate<T>, IRemove<T>, IGetById<T>, IGetMany<T>, ICount<T>
    {
    }
}