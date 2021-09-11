using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Core.BaseAbstractions;
using Infrastructure.Data.Migrations;
using Models.Db.Relations;

namespace Infrastructure.Abstractions
{
    using T = DeskShare;

    public interface IDeskShareRepository : IAdd<T>, IGetOne<T>, IAddMany<T>, IRemove<T>, IRemoveMany<T>
    {
        Task<bool> HasSharedWriteTo(long id, long recipientId);
        
        Task<bool> HasSharedReadTo(long id, long recipientId);

        Task<ICollection<T>> GetShares(long id);

        Task<ICollection<long>> GetIndividuallyShared(long accountId);
    }
}