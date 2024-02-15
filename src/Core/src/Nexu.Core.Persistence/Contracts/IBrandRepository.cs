using System;
using System.Collections.Generic;
using Nexu.Core.Domain.Entities;
using Nexu.Shared.Infrastructure.Persistence;

namespace Nexu.Core.Persistence.Contracts
{
    public interface IBrandRepository : IRepositoryBase
    {
        public List<Model> GetAllModelsByBrandId(int BrandId);
    }
}
