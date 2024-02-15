using System.Threading;
using System.Threading.Tasks;

namespace Nexu.Shared.Infrastructure
{
    /// <summary>
    /// Represents a service that acts as an HTTP client to communicate with another (micro)service.
    /// </summary>
    public interface IHttpProxy
    {
        /// <summary>
        /// Verifies if the remote service is reachable.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> HealthCheck(CancellationToken cancellationToken);
    }
}
