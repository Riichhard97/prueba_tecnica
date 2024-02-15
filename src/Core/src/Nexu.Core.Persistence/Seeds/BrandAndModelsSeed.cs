using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json;
using Nexu.Core.Domain.Entities;
using Nexu.Shared.EntityFrameworkCore;
using Nexu.Shared.Infrastructure.Persistence;

namespace Nexu.Core.Persistence.Seeds
{
    public class BrandAndModelsSeed : ISeed
    {
        private readonly IRepository _repository;
        private readonly CoreDbContext _coreDbContext;

        public BrandAndModelsSeed(IRepository repository, CoreDbContext coreDbContext)
        {
            _repository = repository;
            _coreDbContext = coreDbContext;
        }

        public async Task Init()
        {
            if(!await CheckIfNeedRunSeed())
            {
                await RunSeed();
            }
        }

        private async Task<bool> CheckIfNeedRunSeed()
        {
            return await _repository.AnyAsync<Brand>();
        }

        private async Task RunSeed()
        {
            var serializer = new JsonSerializer();
            List<JsonModel> models = new();
            using (var streamReader = new StreamReader("./models.json"))
            using (var textReader = new JsonTextReader(streamReader))
            using (var transaction = await _repository.BeginTransactionAsync())
            {
                try
                {
                    models = serializer.Deserialize<List<JsonModel>>(textReader);

                    foreach (var model in models)
                    {
                        var brand = await GetOrCreateIfNotExistBrand(model.brand_name);
                        await CreateModel(model, brand.Id);
                    }
                    await _coreDbContext.Database.ExecuteSqlRawAsync("SELECT SETVAL('public.\"Model_Id_seq\"', (SELECT MAX(\"Id\") FROM public.\"Model\"));");
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }

        }

        private async Task<Brand> GetOrCreateIfNotExistBrand(string name)
        {
            Brand brand;
            brand = await _repository.FirstAsync<Brand>(x=>x.Name == name);

            if(brand == null)
            {
                brand = new Brand() { Name = name};
                brand = await _repository.AddAsync(brand);
            }
            return brand;
        }

        private async Task CreateModel(JsonModel model, int BrandId)
        {
            Model objModel = new Model() {Id = model.id, Name = model.name, BrandId = BrandId, AveragePrice = model.average_price};
            _repository.Add(objModel);
        }
    }
}
