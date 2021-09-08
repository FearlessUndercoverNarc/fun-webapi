using System.Threading.Tasks;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IAdd<T>
    {
        Task Add(T entity);
    }
}