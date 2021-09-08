using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IRemoveMany<T>
    {
        Task RemoveMany(ICollection<T> entities);
    }
}