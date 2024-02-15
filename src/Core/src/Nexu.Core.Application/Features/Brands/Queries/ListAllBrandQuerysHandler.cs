using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Domain.Entities;
using Nexu.Shared.Infrastructure.Persistence;
using Nexu.Shared.Infrastructure.Queries;

namespace Nexu.Core.Application.Features.Brands.Queries
{
    public class ListAllBrandQuerysHandler : IQueryHandler<ListAllBrandQuerys, List<BrandDto>>
    {
        private IReadOnlyRepository _readOnlyRepository;
        private readonly IMapper _mapper;

        public ListAllBrandQuerysHandler(IReadOnlyRepository readOnlyRepository, IMapper mapper)
        {
            _readOnlyRepository = readOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<BrandDto>> Handle(ListAllBrandQuerys request, CancellationToken cancellationToken)
        {
            var queryable = _readOnlyRepository.Query<Brand>()
                .Include(x => x.Models);

            var result = queryable.ProjectTo<BrandDto>(_mapper.ConfigurationProvider);


            return result.ToList();
        }
    }
}
