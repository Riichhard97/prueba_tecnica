using Microsoft.Extensions.Logging;
using Nexu.Shared.EntityFrameworkCore;

namespace Nexu.Core.Persistence
{
    public class CoreDbContextMigrator : DbContextMigrator<CoreDbContext>
    {
        public CoreDbContextMigrator(CoreDbContext context, ILogger<CoreDbContextMigrator> logger)
              : base(context, logger)
        {
        }
    }
}
