using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexu.Shared.Infrastructure;
using Nexu.Shared.Infrastructure.Queries;

namespace Nexu.Shared.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        // TODO: Rename to AddInfrastructureServices
        public static void AddInfrastructureServices(this IServiceCollection services, Assembly applicationAssembly)
        {
            services.AddLazy();
            services.AddSingleton<IPaginator, DefaultPaginator>();

            services.AddAutoMapper(applicationAssembly);
            services.AddSingleton<IDateTime>(new SystemDateTime());

            // Add MediatR
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            services.AddMediatR(applicationAssembly);            
        }

        private static IServiceCollection AddAutoMapper(this IServiceCollection services, Assembly applicationAssembly)
        {
            // TODO: Might want to use the nuget package instead
            // https://github.com/AutoMapper/AutoMapper.Extensions.Microsoft.DependencyInjection
            if (services.Any(sd => sd.ServiceType == typeof(IMapper)))
            {
                return services;
            }

            var openTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>),
                typeof(IValueConverter<,>),
                typeof(IMappingAction<,>)
            };

            var allTypes = applicationAssembly.DefinedTypes.ToArray();
            foreach (var type in openTypes.SelectMany(openType => allTypes
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && t.AsType().ImplementsGenericInterface(openType))))
            {
                services.AddTransient(type.AsType());
            }

            var mapperConfiguration = new MapperConfiguration(cfg => AutoMapperConfig.Configure(cfg, applicationAssembly));
            services.AddSingleton<IConfigurationProvider>(mapperConfiguration);
            services.AddSingleton<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));

            return services;
        }

        public static IServiceCollection AddLazy(this IServiceCollection services)
        {
            return services.AddTransient(typeof(Lazy<>), typeof(LazyService<>));
        }

        private class LazyService<T> : Lazy<T>, IDisposable
        {
            private bool _disposed;

            public LazyService(IServiceProvider provider)
                : base(provider.GetRequiredService<T>)
            {
            }

            public void Dispose()
            {
                if (!IsValueCreated)
                {
                    return;
                }

                if (_disposed)
                {
                    return;
                }

                if (Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                _disposed = true;
            }
        }

        private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
            => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        private static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
