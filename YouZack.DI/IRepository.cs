using System.Threading;
using System.Threading.Tasks;

namespace Infrastructures.DI
{
    public interface IRepository
    {

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
