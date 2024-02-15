using Microsoft.Extensions.DependencyInjection;
using Nexu.Shared.RemoteServices.EventHandlers;
using Nexu.Shared.RemoteServices.RequestControllers;
using Microsoft.Extensions.Configuration;
using System;

namespace Nexu.Shared.RemoteServices
{
    public static class RemoteServicesResgistrationExtensions
    {
        public static void AddRemoteServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            var auth = configuration["Services:Auth"];
            var history = configuration["Services:History"];
            var core = configuration["Services:Core"];
            services.AddHttpClient("Auth", config => config.BaseAddress = new Uri(auth));
            services.AddHttpClient("History", config => config.BaseAddress = new Uri(history));
            services.AddHttpClient("Core", config => config.BaseAddress = new Uri(core));
            services.AddHttpClient("Permissions", config => config.BaseAddress = new Uri(auth));

            services.AddScoped<IAuthMicroservice, AuthMicroservice>();
            services.AddScoped<IUserEventHandler, UserEventHandler>();                                    
            services.AddTransient<IHistoryMicroservice, HistoryMicroservice>();
            services.AddTransient<ICoreMicroservice, CoreMicroservice>();            
            services.AddTransient<IPermissionsMicroservice, PermissionsMicroservice>();
        }      
        
    }
}




