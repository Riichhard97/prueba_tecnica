using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Domain.Entities;
using Nexu.Shared.AspNetCore.Exceptions;
using Nexu.Shared.Exceptions;
using Nexu.Shared.Infrastructure.Persistence;

namespace Nexu.Core.Application.Features.Models.Commands
{
    public class PutModelCommandHandler : IRequestHandler<PutModelCommand, ModelDto>
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public PutModelCommandHandler(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ModelDto> Handle(PutModelCommand request, CancellationToken cancellationToken)
        {
            await ValidateRequest(request);

            var model = await _repository.FirstAsync<Model>(x => x.Id == request.id);
            model.AveragePrice = request.average_price;

            model = _repository.Update(model);

            await _repository.SaveChangesAsync();
            var modelDto = _mapper.Map<ModelDto>(model);

            return modelDto;
        }

        public async Task ValidateRequest(PutModelCommand request)
        {
            var existingBrand = await _repository.FirstAsync<Model>(x => x.Id == request.id);
            if (existingBrand == null)
                throw new BusinessLogicException("El modelo id proporcionado no existe");

            if (request.average_price < 100000)
                throw new BusinessLogicException("El average price debe ser mayor a 100,000.");
        }
    }
}
