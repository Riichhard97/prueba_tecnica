using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Nexu.Core.Application.Brands.Mappings;
using Nexu.Core.Application.Features.Brands.Commands;
using Nexu.Core.Application.Features.Brands.Dtos;
using Nexu.Core.Persistence;
using Nexu.Shared.AspNetCore.Exceptions;
using Nexu.Shared.EntityFrameworkCore;
using Nexu.Shared.Infrastructure.Persistence;
using Nexu.Core.Test.Mocks;
using Xunit;
using Nexu.Shared.Exceptions;

namespace Nexu.Core.Test.Features
{
    public class ValidatesNegRulesBrand
    {
        private readonly IMapper _mapper;
        private readonly IRepository _repository;

        public ValidatesNegRulesBrand() {
            var _context = MockCoreDbContext.GetSingleRepository();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<ListModelToModelDto>();
                c.AddProfile<ListBrandToBrandDto>();
            });
            _repository = new Mock<EfRepository<CoreDbContext>>(_context).Object;
            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task AddBrandOnlyWithName()
        {
            var handler = new AddBrandCommandHandler(_repository, _mapper);
            var result = await handler.Handle(new AddBrandCommand() { name="TEST"}, new System.Threading.CancellationToken());
            // Assert
            Assert.True(result.id > 0);
        }
        public async Task<BrandDto> AddBrand()
        {
            var handler = new AddBrandCommandHandler(_repository, _mapper);
            var result = await handler.Handle(new AddBrandCommand() { name = "TEST" }, new System.Threading.CancellationToken());

            return result;
        }

        [Fact]
        public void AddModelWithUnknownBrandThrowError()
        {
            try
            {
                int brandId = 99999;

                var handler = new AddModelCommandHandler(_repository, _mapper);

                var modelCommand = new AddModelCommand() { name = "TEST", average_price = 55232323, brand_id = brandId };
                var result = handler.Handle(modelCommand, new System.Threading.CancellationToken());
            }
            catch (BusinessLogicException ex)
            {
                Assert.Equal("El brand id proporcionado no existe", ex.Message);
            }
        }

        [Fact]
        public void AddModelWithLessThan100000InAveragePrice()
        {
            try
            {
                var brand = AddBrand();
                var handler = new AddModelCommandHandler(_repository, _mapper);

                var modelCommand = new AddModelCommand() { name = "TEST", average_price = 50000, brand_id = brand.Id };
                var result = handler.Handle(modelCommand, new System.Threading.CancellationToken());
            }
            catch (BusinessLogicException ex)
            {
                Assert.Equal("El average price debe ser mayor a 100,000.", ex.Message);
            }
        }

        [Fact]
        public void AddModelWithNameRepeatedInTheSameBrand()
        {
            string sameName = "TEST";
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    var brand = AddBrand();
                    var handler = new AddModelCommandHandler(_repository, _mapper);

                    var modelCommand = new AddModelCommand() { name = sameName, average_price = 9999999, brand_id = brand.Id };
                    var result = handler.Handle(modelCommand, new System.Threading.CancellationToken());
                }
            }
            catch (BusinessLogicException ex)
            {
                Assert.Equal("El nombre proporcionado ya existe en otro modelo dentro de la misma marca.", ex.Message);
            }
        }
    }
}
