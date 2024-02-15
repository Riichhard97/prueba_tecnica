using Microsoft.EntityFrameworkCore;
using Nexu.Shared.EntityFrameworkCore;
using Nexu.Shared.Infrastructure;

namespace Nexu.Core.Persistence
{
    public class CoreDbContext : DbContextBase
    {
        public CoreDbContext(
            DbContextOptions<CoreDbContext> options,
            IDateTime clock,
            ICurrentUserAccessor currentUserAccessor
        ) : base(options, clock, currentUserAccessor)
        {

        }
    }
}
