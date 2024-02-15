using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Domain.Entities;
using Nexu.Shared.Infrastructure.Persistence;

namespace Nexu.Core.Application.Features.Brands.Commands
{
    public class AddBrandCommandHandler : MediatR.IRequestHandler<AddBrandCommand, BrandDto>
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        public AddBrandCommandHandler(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<BrandDto> Handle(AddBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _repository.AddAsync(new Brand() { Name = request.name });

            var brandDto = _mapper.Map<BrandDto>(brand);
            return brandDto;
        }
    }
}
