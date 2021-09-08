using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Db.Common;

namespace Infrastructure.Core.BaseImplementations
{
    public class VersionedRepositoryBase<T> : IdRepositoryBase<T> where T : VersionedEntity
    {
        protected VersionedRepositoryBase(FunDbContext context) : base(context)
        {
        }

        public async Task<T> GetLastVersion()
        {
            return await GetDbSetT().OrderByDescending(t => t.Version).FirstOrDefaultAsync();
        }
    }
}