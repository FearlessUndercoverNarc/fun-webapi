using Infrastructure.Core.BaseAbstractions;
using Models.Db.Tree;

namespace Infrastructure.Abstractions
{
    using T = Desk;

    public interface IDeskRepository : IAdd<T>, IGetMany<T>, IGetById<T>, IUpdate<T>
    {
    }
}