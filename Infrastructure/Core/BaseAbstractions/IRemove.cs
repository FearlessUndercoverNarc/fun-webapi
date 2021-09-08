using System.Threading.Tasks;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IRemove<T>
    {
        Task Remove(T entity);
    }
}