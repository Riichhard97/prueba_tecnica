using MediatR;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Domain.Entities;

namespace Nexu.Core.Application.Features.Models.Commands
{
    public class PutModelCommand : IRequest<ModelDto>
    {
        public int id { get; set; }
        public int average_price { get; set; }
    }
}
