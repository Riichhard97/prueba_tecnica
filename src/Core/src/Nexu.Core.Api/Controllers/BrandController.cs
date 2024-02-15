using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexu.Core.Application.Features.Brands.Commands;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Application.Features.Brands.Queries;
using Nexu.Core.Domain.Entities;

namespace Nexu.Core.Api.Controllers
{
    [ApiController]
    [Route("brands")]
    public class BrandController : ControllerBase
    {
        private readonly ISender _sender;

        public BrandController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet()]
        public async Task<List<BrandDto>> GetAll([FromQuery] ListAllBrandQuerys query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            return result;
        }

        [HttpGet("{id}/models")]
        public async Task<List<ModelDto>> GetModelsByBrandId(int Id, CancellationToken cancellationToken)
        {
            var ListModelsByBrandIdQuerys = new ListModelsByBrandIdQuerys();
            ListModelsByBrandIdQuerys.BrandId = Id;
            var result = await _sender.Send(ListModelsByBrandIdQuerys, cancellationToken);
            return result;
        }

        [HttpPost()]
        public async Task<BrandDto> AddBrand(AddBrandCommand query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            return result;
        }

        [HttpPost("{id}/models")]
        public async Task<ModelDto> AddModel(int Id, AddModelCommand query, CancellationToken cancellationToken)
        {
            query.brand_id = Id;
            var result = await _sender.Send(query, cancellationToken);
            return result;
        }
    }
}
