using System.Threading.Tasks;
using Infrastructure.Core.BaseAbstractions;
using Models.Db.Tree;

namespace Infrastructure.Abstractions
{
    using T = DeskActionHistoryItem;

    public interface IDeskActionHistoryRepository : IAdd<T>, IUpdate<T>, IRemove<T>, IGetMany<T>, IGetOne<T>, IGetLastVersion<T>, IGetById<T>
    {
        Task<uint> GetLastVersionByDesk(long deskId);
    }
}