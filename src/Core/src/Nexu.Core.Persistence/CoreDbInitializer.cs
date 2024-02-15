using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nexu.Core.Persistence.Seeds;
using Nexu.Shared.EntityFrameworkCore;
using Nexu.Shared.Infrastructure.Persistence;

namespace Nexu.Core.Persistence
{

    public class CoreDbInitializer
    {
        private readonly CoreDbContext _context;
        private readonly IRepository _repository;

        public CoreDbInitializer(CoreDbContext context, IRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task Run()
        {
            if (_context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                await _context.Database.MigrateAsync();
            }

            await new BrandAndModelsSeed(_repository, _context).Init();
        }

    }
}
