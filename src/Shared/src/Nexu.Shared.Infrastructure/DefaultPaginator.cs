using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nexu.Shared.Infrastructure.Persistence;
using Nexu.Shared.Infrastructure.Queries;

namespace Nexu.Shared.Infrastructure
{
    public class DefaultPaginator : IPaginator
    {
        public async Task<PaginatedResult<TItem>> MakePageAsync<TCount, TItem>(IRepositoryBase repository, IQueryable<TCount> countQuery,
            IQueryable<TItem> itemsQuery, int page, int pageSize, bool generatePaginator, CancellationToken cancellationToken = default)
            where TCount : class where TItem : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (countQuery is null)
            {
                throw new ArgumentNullException(nameof(countQuery));
            }

            if (itemsQuery is null)
            {
                throw new ArgumentNullException(nameof(itemsQuery));
            }

            int count = 0;
            List<TItem> items = null;
            if (generatePaginator)
            {
                Paginator.ValidatePaging(page, pageSize);
                count = await repository.CountAsync(countQuery, cancellationToken).ConfigureAwait(false);
                items = await repository.ListAsync(itemsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                items = await repository.ListAsync(itemsQuery, cancellationToken).ConfigureAwait(false);
                count = items.Count();
            }

            return PaginatedResult.From(items, count, page, pageSize);
        }
    }
}
