using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Core.BaseAbstractions;
using Models.Db.Relations;

namespace Infrastructure.Abstractions
{
    using T = FolderShare;

    public interface IFolderShareRepository : IAdd<T>, IGetOne<T>, IAddMany<T>, IRemoveMany<T>
    {
        Task<bool> HasSharedWriteTo(long id, long recipientId);

        Task<bool> HasSharedReadTo(long id, long recipientId);

        Task<ICollection<T>> GetShares(long id);

        Task<ICollection<long>> GetSharedRoots(long accountId);
    }
}