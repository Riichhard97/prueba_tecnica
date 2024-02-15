using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PA.Genety.Shared.SignalR.Hubs;

namespace PA.Genety.Shared.SignalR
{
    public static class SignalRServiceRegistrationExtensions
    {    
        public static IServiceCollection AddGenetySignalR(this IServiceCollection services, 
        IWebHostEnvironment environment)
        {
            if(environment.IsDevelopment()){
                services.AddSignalR(hubOptions =>
                {
                    hubOptions.EnableDetailedErrors = true;
                });    
            }else{
                services.AddSignalR(hubOptions =>
                {
                    hubOptions.EnableDetailedErrors = true;
                })
                .AddAzureSignalR();
            }                        
            return services;
        }

        public static void ConfigureHubs(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<ChatHub>("hubs/chat");
            endpoints.MapHub<MessageBrokerHub>("hubs/messagebroker");
        }    
    }
}
