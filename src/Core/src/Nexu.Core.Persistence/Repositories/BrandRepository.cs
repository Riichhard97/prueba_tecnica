using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nexu.Core.Domain.Entities;
using Nexu.Core.Persistence.Contracts;
using Nexu.Shared.EntityFrameworkCore;

namespace Nexu.Core.Persistence.Repositories
{
    public class BrandRepository<TContext> : ReadOnlyEfRepository, IBrandRepository
            where TContext : DbContext
    {
        public BrandRepository(TContext context)
            : base(context) { }

        public List<Model> GetAllModelsByBrandId(int BrandId)
        {
            var AllModels = Context.Set<Model>().Where(x => x.BrandId == BrandId);

            return AllModels.ToList();
        }
    }
}
