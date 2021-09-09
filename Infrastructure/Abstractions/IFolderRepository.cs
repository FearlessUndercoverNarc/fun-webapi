using Infrastructure.Core.BaseAbstractions;
using Models.Db.Tree;

namespace Infrastructure.Abstractions
{
    using T = Folder;

    public interface IFolderRepository : IAdd<T>, IUpdate<T>, IGetMany<T>, IGetById<T>
    {
    }
}