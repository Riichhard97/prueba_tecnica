using System.Collections.Generic;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Domain.Entities;
using Nexu.Shared.Infrastructure.Queries;

namespace Nexu.Core.Application.Features.Models.Queries
{
    public class ListModelsByFilterQuerys : IQuery<List<ModelDto>>
    {
        public int? greater { get; set; }
        public int? lower { get; set; }
    }
}
