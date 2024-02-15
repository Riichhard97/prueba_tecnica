using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Domain.Entities;
using Nexu.Shared.Infrastructure.Persistence;
using Nexu.Shared.Infrastructure.Queries;

namespace Nexu.Core.Application.Features.Models.Queries
{
    public class ListModelsByFilterQuerysHandler : IQueryHandler<ListModelsByFilterQuerys, List<ModelDto>>
    {
        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IMapper _mapper;

        public ListModelsByFilterQuerysHandler(IReadOnlyRepository readOnlyRepository, IMapper mapper)
        {
            _readOnlyRepository = readOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<ModelDto>> Handle(ListModelsByFilterQuerys request, CancellationToken cancellationToken)
        {
            var query = _readOnlyRepository.Query<Model>();
                
            if (request.greater.HasValue)
                query = query.Where(x => x.AveragePrice > request.greater);

            if (request.lower.HasValue)
                query = query.Where(x => x.AveragePrice < request.lower);


            return query.ProjectTo<ModelDto>(_mapper.ConfigurationProvider).ToList();
        }
    }
}
