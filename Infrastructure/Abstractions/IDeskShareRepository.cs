using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Core.BaseAbstractions;
using Models.Db.Relations;

namespace Infrastructure.Abstractions
{
    using T = DeskShare;

    public interface IDeskShareRepository : IAdd<T>, IGetOne<T>, IAddMany<T>, IRemove<T>, IRemoveMany<T>
    {
        Task<bool> IsSharedTo(long id, long recipientId);

        Task<ICollection<long>> GetIndividuallyShared(long accountId);
    }
}