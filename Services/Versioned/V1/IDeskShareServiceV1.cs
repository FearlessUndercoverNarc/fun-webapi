using System.Threading.Tasks;

namespace Services.Versioned.V1
{
    public interface IDeskShareServiceV1
    {
        Task Share(long id, long recipientId);

        Task RemoveShare(long id, long recipientId);
    }
}