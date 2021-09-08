using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IAddMany<T>
    {
        Task AddMany(ICollection<T> entities);
    }
}