using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nexu.Shared.Infrastructure.Persistence
{
    public interface IRepositoryTransaction : IDisposable, IAsyncDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default);

        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
