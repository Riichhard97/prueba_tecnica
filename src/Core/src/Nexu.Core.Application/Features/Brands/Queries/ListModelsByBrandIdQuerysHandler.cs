using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Nexu.Core.Application.Brands.Mappings;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Application.Features.Brands.Queries;
using Nexu.Core.Domain.Entities;
using Nexu.Shared.Infrastructure.Persistence;
using Nexu.Shared.Infrastructure.Queries;

namespace Nexu.Core.Application.Features.Brands.Queries
{
    public class ListModelsByBrandIdQuerysHandler : IQueryHandler<ListModelsByBrandIdQuerys, List<ModelDto>>
    {
        private IReadOnlyRepository _readOnlyRepository;
        private readonly IMapper _mapper;

        public ListModelsByBrandIdQuerysHandler(IReadOnlyRepository readOnlyRepository, IMapper mapper)
        {
            _readOnlyRepository = readOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<ModelDto>> Handle(ListModelsByBrandIdQuerys request, CancellationToken cancellationToken)
        {
            var queryable = _readOnlyRepository.Query<Model>().Where(x=>x.BrandId == request.BrandId);
            var result = queryable.ProjectTo<ModelDto>(_mapper.ConfigurationProvider).OrderBy(x=>x.name);

            return result.ToList();
        }
    }
}
