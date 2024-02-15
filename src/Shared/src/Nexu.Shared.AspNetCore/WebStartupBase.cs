using System;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Nexu.Shared.AspNetCore.Authentication;
using Nexu.Shared.AspNetCore.Authorization;
using Nexu.Shared.AspNetCore.Filters;
using Nexu.Shared.AspNetCore.Middleware;
using Nexu.Shared.AspNetCore;
using Nexu.Shared.Infrastructure.Authorization;
using Nexu.Shared.Infrastructure.Json;
using Nexu.Shared.Infrastructure.Security;
using Nexu.Shared.Infrastructure;
using Nexu.Shared;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Nexu.Shared.AspNetCore
{
    public abstract class WebStartupBase
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public ILogger Logger { get; }

        protected abstract Assembly ApplicationAssembly { get; }

        protected WebStartupBase(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
            Logger = configuration.CreateDefaultLogger(GetType());
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                var mapper = app.ApplicationServices.GetService<IMapper>();
                mapper.ConfigurationProvider.AssertConfigurationIsValid();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });
            ConfigureApp(app);

            app.UseEndpoints(endpoints =>
            {
                var allowAnonymous = new AllowAnonymousAttribute();
                endpoints.MapHealthChecks("/health").WithMetadata(allowAnonymous);
                // The readiness check uses all registered checks with the 'ready' tag.
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                }).WithMetadata(allowAnonymous);

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    // Exclude all checks and return a 200-Ok.
                    Predicate = (_) => false
                }).WithMetadata(allowAnonymous);

                ConfigureEndpoints(endpoints);
                ConfigureHubs(endpoints);
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog());
            AddWebServices(services);
            AddAppServices(services);
        }

        /// <summary>
        /// Registers all the ASP.NET-specific services.
        /// </summary>
        /// <param name="services"></param>
        protected virtual void AddWebServices(IServiceCollection services)
        {
            AddHealthChecks(services);
            AddMvc(services);


            services.AddCors(Configuration, HeaderNames.ContentDisposition, "x-rate-limit-limit", "x-rate-limit-remaining", "x-rate-limit-reset", "x-account");

            if (Environment.IsDevelopment())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                });
                //services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
                services.AddControllers();
            }
        }

        /// <summary>
        /// Registers all the application-level services. Override this method to add the app-specific services.
        /// </summary>
        /// <param name="services"></param>
        protected virtual void AddAppServices(IServiceCollection services)
        {
            services.AddInfrastructureServices(ApplicationAssembly);

            services.AddValidatorsFromAssemblies(new[]
            {
                ApplicationAssembly,
                // TODO: Add common validator assemblies
                // typeof(ValidatorType).Assembly,
            }, lifetime: ServiceLifetime.Singleton);

            services.AddDiagnosticContextAccessors();

            services.AddCustomExceptionHandler<NoCurrentAccountExceptionHandler>();

        }

        protected virtual IHealthChecksBuilder AddHealthChecks(IServiceCollection services)
        {
            return services.AddHealthChecks();
        }

        protected virtual IMvcCoreBuilder AddMvc(IServiceCollection services)
        {
            var instrumentationKey = Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
            if (!string.IsNullOrEmpty(instrumentationKey))
            {
                services.AddApplicationInsightsTelemetry(instrumentationKey);
                services.AddLogging(builder =>
                {
                    builder.AddApplicationInsights(instrumentationKey);
                });
            }

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressMapClientErrors = true;
            });
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            // Register accessor to ClaimsPrincipal
            services.AddScoped(x => x.GetRequiredService<IHttpContextAccessor>().HttpContext.User);

            var builder = services.AddMvcCore(options =>
            {
                options.Filters.Add(new ExceptionHandlerFilter());
                options.Filters.Add(new CurrentContextActionFilter());
                //options.Filters.Add(new RequireActiveWorkspaceFilter());                    
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddFluentValidation()
                .AddJsonOptions(o =>
                {
                    var options = o.JsonSerializerOptions;
                    options.IgnoreNullValues = true;
                    options.Converters.Add(new UtcDateTimeJsonConverter());
                }).AddRazorViewEngine();


            return builder;
        }

        protected virtual void ConfigureApp(IApplicationBuilder app)
        {
            app.UseRequestLocalization("en-US", Languages.Spanish);

            app.UseSerilogRequestLogging(options =>
            {
                options.IgnoreHealthchecks();
                options.EnrichDiagnosticContext = EnrichFromRequest;
            });

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseRouting();
            app.UseCors();
            app.UseHeaderPropagation();
        }

       
        protected virtual void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();
        }

        protected virtual void ConfigureHubs(IEndpointRouteBuilder endpoints)
        { }


        private static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var userAgent = httpContext.Request.Headers[HeaderNames.UserAgent];
            if (userAgent.Count > 0)
            {
                diagnosticContext.Set("UserAgent", userAgent[0]);
            }

            if (httpContext.Connection.RemoteIpAddress is not null)
            {
                diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress.ToString());
            }

            var language = httpContext.Request.Headers[HeaderNames.AcceptLanguage];
            if (language.Count > 0)
            {
                diagnosticContext.Set("Language", language[0]);
            }
        }
    }
}
