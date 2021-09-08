using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Core.BaseAbstractions
{
    public interface IGetAllAsDictionary<T>
    {
        public Task<Dictionary<long, T>> GetAllAsDictionary();
    }
}