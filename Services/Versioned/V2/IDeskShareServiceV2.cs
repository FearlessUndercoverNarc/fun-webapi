using System.Threading.Tasks;

namespace Services.Versioned.V2
{
    public interface IDeskShareServiceV2
    {
        Task Share(long id, long recipientId);

        Task RemoveShare(long id, long recipientId);
    }
}