using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Domain.Entities;
using Nexu.Shared.AspNetCore.Exceptions;
using Nexu.Shared.Exceptions;
using Nexu.Shared.Infrastructure;
using Nexu.Shared.Infrastructure.Persistence;

namespace Nexu.Core.Application.Features.Brands.Commands
{
    public class AddModelCommandHandler : IRequestHandler<AddModelCommand, ModelDto>
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        public AddModelCommandHandler(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<ModelDto> Handle(AddModelCommand request, CancellationToken cancellationToken)
        {
            var existingBrand = await _repository.FirstAsync<Brand>(x => x.Id == request.brand_id);
            if (existingBrand == null)
                throw new BusinessLogicException("El brand id proporcionado no existe");

            var existingModel = await _repository.FirstAsync<Model>(x => x.Name == request.name && x.BrandId == request.brand_id);
            if (existingModel != null)
                throw new BusinessLogicException("El nombre proporcionado ya existe en otro modelo dentro de la misma marca.");
            if(request.average_price != null)
            {
                if (request.average_price < 100000)
                   throw new BusinessLogicException("El average price debe ser mayor a 100,000.");
            }


            var allModels = _repository.Query<Model>().OrderByDescending(x => x.Id);
            var lastItem = await allModels.FirstAsync();

            var model = await _repository.AddAsync(new Model()
            {
                Name = request.name,
                AveragePrice = request.average_price.HasValue ? request.average_price.Value : 0,
                BrandId = request.brand_id.Value
            });

            var modelDto = _mapper.Map<ModelDto>(model);
            return modelDto;
        }
    }
}
