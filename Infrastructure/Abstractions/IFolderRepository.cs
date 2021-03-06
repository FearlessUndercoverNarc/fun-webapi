using System.Threading.Tasks;
using Infrastructure.Core.BaseAbstractions;
using Models.Db.Tree;

namespace Infrastructure.Abstractions
{
    using T = Folder;

    public interface IFolderRepository : IAdd<T>, IUpdate<T>, IGetMany<T>, IGetById<T>, IRemove<T>, IRemoveMany<T>, ICount<T>
    {
        Task<bool> IsParentSharedTo(long id, long accountId);
    }
}