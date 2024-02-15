using Microsoft.EntityFrameworkCore;
using Nexu.Shared.Infrastructure.Persistence;

namespace Nexu.Shared.EntityFrameworkCore
{
    public class ReadOnlyEfRepository : EfRepositoryBase, IReadOnlyRepository
    {
        public ReadOnlyEfRepository(DbContext context)
            : base(context, true)
        {
        }
    }

    public class ReadOnlyEfRepository<TContext> : ReadOnlyEfRepository
        where TContext : DbContext
    {
        public ReadOnlyEfRepository(TContext context)
            : base(context)
        {
        }
    }
}
