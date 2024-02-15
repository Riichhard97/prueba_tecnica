using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Domain.Entities;

namespace Nexu.Core.Application.Features.Brands.Commands
{
    public class AddBrandCommand : IRequest<BrandDto>
    {
        public string name { get; set; }
    }
}
