using System.Threading.Tasks;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IGetLastVersion<T>
    {
        Task<T> GetLastVersion();
    }
}