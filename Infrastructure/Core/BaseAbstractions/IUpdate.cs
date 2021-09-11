using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IUpdate<T>
    {
        Task Update(T entity);

        Task UpdateMany(ICollection<T> entities);
    }
}