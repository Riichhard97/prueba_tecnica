using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nexu.Shared.Infrastructure.Persistence;

namespace Nexu.Shared.Infrastructure.Queries
{
    public interface IPaginator
    {
        Task<PaginatedResult<TItem>> MakePageAsync<TCount, TItem>(IRepositoryBase repository, IQueryable<TCount> countQuery,
            IQueryable<TItem> itemsQuery, int page, int pageSize, bool generatePaginator, CancellationToken cancellationToken = default)
            where TCount : class where TItem : class;
    }
}
