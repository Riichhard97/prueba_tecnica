

using System.Collections.Generic;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Shared.Infrastructure.Queries;

namespace Nexu.Core.Application.Features.Brands.Queries
{
    public class ListModelsByBrandIdQuerys : IQuery<List<ModelDto>>
    {
        public int BrandId { get; set; }
    }
}
