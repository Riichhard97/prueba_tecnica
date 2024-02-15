using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Nexu.Core.Application.Features.Models.Commands;
using Nexu.Core.Application.Features.Models.Queries;
using System.Collections.Generic;
using Nexu.Core.Application.Features.Brands.Dtos;

namespace Nexu.Core.Api.Controllers
{
    [ApiController]
    [Route("models")]
    public class ModelController : ControllerBase
    {
        private readonly ISender _sender;

        public ModelController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet()]
        public async Task<List<ModelDto>> GetAllByFilters([FromQuery] ListModelsByFilterQuerys query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            return result;
        }

        [HttpPut("{id}")]
        public async Task<ModelDto> UpdateModel(int id, PutModelCommand query, CancellationToken cancellationToken)
        {
            query.id = id;
            var result = await _sender.Send(query, cancellationToken);
            return result;
        }
    }
}
