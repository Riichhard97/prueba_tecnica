using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexu.Core.Persistence;
using Nexu.Shared.AspNetCore;
using Nexu.Core.Application.Features;
using Nexu.Shared.EntityFrameworkCore;
using Nexu.Shared.Infrastructure;
using Serilog;

namespace Nexu.Core.Api
{
    public class Startup : WebStartupBase
    {

        private readonly IWebHostEnvironment _environment;
        private readonly string _filesPath;
        protected override Assembly ApplicationAssembly => typeof(GeneralMappings).Assembly;

        public Startup(IConfiguration configuration,
            IWebHostEnvironment environment)
          : base(configuration, environment)
        {
            _environment = environment;
            var filesPath = Configuration["FilesPath"];
            if (!string.IsNullOrEmpty(filesPath))
            {
                // Files are going to be saved to the local environment                
                _filesPath = System.Environment.ExpandEnvironmentVariables(filesPath);
                if (!Path.IsPathRooted(_filesPath))
                {
                    _filesPath = Path.Combine(environment.ContentRootPath, _filesPath);
                }
            }
        }

        protected override void AddAppServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog());
            base.AddAppServices(services);

            var connectionString = Configuration.GetConnectionString("App");
            services.AddDbContext<CoreDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                if (Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }
                options.DisableRequiredNavigationWithQueryFilterInteractionWarning();
            });

            services.AddTransient<CoreDbContextMigrator>();
            services.AddTransient<CoreDbInitializer>();
            services.AddRepository<CoreDbContext>();
            services.AddCoreRepository<CoreDbContext>(); 
            //services.AddAplicationServices();     
            //services.AddRemoteServices(Configuration);        
            
            //TODO: Change this implementation for apigateway segurity
            // services.AddMvcCore(options => {
            //     options.Filters.Add(new AccountActiveHandler());
            // });

            var supportEmail = Configuration.GetRequiredValue<string>("SupportEmail");
            services.AddSingleton(new SupportConfiguration { Email = supportEmail });
            if (_environment.IsDevelopment())
            {
                Logger.LogDebug("Using files path {FilesPath}", _filesPath);
                services.AddSingleton<IFileSystem>(new LocalFileSystem(_filesPath));
            }
            else
            {
                var storageConnectionString = Configuration["Azure:StorageAccount:ConnectionString"];
                var containerName = Configuration["Azure:StorageAccount:ContainerName"];

                services.AddSingleton<IFileSystem>(new AzureFileSystem(storageConnectionString, containerName));
            }           

            services.AddDefaultHeaderPropagation();           
        }        

    }
}
