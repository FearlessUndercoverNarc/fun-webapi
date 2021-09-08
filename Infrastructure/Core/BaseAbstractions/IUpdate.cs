using System.Threading.Tasks;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IUpdate<in T>
    {
        Task Update(T entity);
    }
}