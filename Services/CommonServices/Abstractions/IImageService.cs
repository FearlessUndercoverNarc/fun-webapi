using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.CommonServices.Abstractions
{
    public interface IImageService
    {
        Task<string> Create(string filename, string folder, byte[] data);

        Task<ICollection<string>> Enlist(string folder);
    }
}