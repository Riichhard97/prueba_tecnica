
using MediatR;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Domain.Entities;

namespace Nexu.Core.Application.Features.Brands.Commands
{
    public class AddModelCommand : IRequest<ModelDto> 
    { 
        public string name { get; set; }
        public int? average_price { get; set; }
        public int? brand_id { get; set; }
    }
}
