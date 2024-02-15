using System;
using System.Reflection;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.ActiveMqTransport;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using MassTransit.Registration;
using MassTransit.Scheduling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nexu.Shared.MassTransit
{
    public static class MassTransitRegistrationExtensions
    {
        private static readonly ushort PrefetchCount =
#if DEBUG
            1;

#else
            (ushort)System.Environment.ProcessorCount;
#endif

        public static void AddMassTransit(this IServiceCollection services, string queueName, IConfiguration configuration, Assembly applicationAssembly,
            Uri scheduler = null, Action<IServiceCollectionBusConfigurator> configure = null)
        {
            var massTransitConfiguration = configuration.GetRequiredSection("MassTransit").Get<MassTransitConfiguration>();

            if (massTransitConfiguration.RabbitMQ == null && massTransitConfiguration.ActiveMQ == null && massTransitConfiguration.AzureServiceBus == null && !massTransitConfiguration.UseInMemory)
            {
                throw new AppConfigurationException("RabbitMQ, ActiveMQ and AzureServiceBus host addresses missing.");
            }

            void UsingActiveMq(IBusRegistrationContext context, IActiveMqBusFactoryConfigurator cfg)
            {
                //cfg.UseHealthCheck(context);

                cfg.Host(massTransitConfiguration.ActiveMQ, h =>
                {
                    h.Username(massTransitConfiguration.Username);
                    h.Password(massTransitConfiguration.Password);

                    var failoverHosts = massTransitConfiguration.GetFailoverHosts();
                    if (failoverHosts != null)
                    {
                        h.FailoverHosts(failoverHosts);
                    }
                });

                cfg.ReceiveEndpoint(queueName, ep =>
                {
                    ep.PrefetchCount = PrefetchCount;
                    ConfigureEndpoint(ep, context);
                });

                Configure(context, cfg, scheduler);
            }

            void UsingRabbitMq(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg)
            {
                // cfg.UseHealthCheck(context);

                cfg.Host(massTransitConfiguration.RabbitMQ, "/", h =>
                {
                    h.Username(massTransitConfiguration.Username);
                    h.Password(massTransitConfiguration.Password);
                });

                cfg.ReceiveEndpoint(queueName, ep =>
                {
                    ep.PrefetchCount = PrefetchCount;
                    ConfigureEndpoint(ep, context);
                });

                Configure(context, cfg, scheduler);
            }

            void UsingInMemory(IBusRegistrationContext context, IInMemoryBusFactoryConfigurator cfg)
            {
                cfg.ReceiveEndpoint(queueName, ep =>
                {
                    ConfigureEndpoint(ep, context);
                });

                Configure(context, cfg, scheduler);
            }

            services.AddMassTransit(x =>
            {
                configure?.Invoke(x);

                if (scheduler is not null)
                {
                    x.AddScopedMessageScheduler(scheduler);
                }

                x.AddConsumers(applicationAssembly);

                if (massTransitConfiguration.AzureServiceBus != null)
                {
                    var connectionString = configuration["Azure:ServiceBus:ConnectionString"];
                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host(connectionString);
                    });
                }
                else if (massTransitConfiguration.ActiveMQ != null)
                {
                    x.UsingActiveMq(UsingActiveMq);
                }
                else if (massTransitConfiguration.RabbitMQ != null)
                {
                    x.UsingRabbitMq(UsingRabbitMq);
                }
                else
                {
                    x.UsingInMemory(UsingInMemory);
                }
            });

            services.AddMassTransitHostedService();
        }

        private static void Configure(IBusRegistrationContext context, IBusFactoryConfigurator cfg, Uri scheduler)
        {
            cfg.UseServiceScope(context);
            var filter = new SendAccountPipeFilter(context);
            cfg.ConfigureSend(x => x.UseFilter(filter));
            cfg.ConfigurePublish(x => x.UseFilter(filter));

            if (scheduler is not null)
            {
                cfg.UseMessageScheduler(scheduler);
            }
        }

        private static void ConfigureEndpoint(IReceiveEndpointConfigurator ep, IBusRegistrationContext context)
        {
            ep.UseMessageRetry(r => r.None());

            ep.ConfigureConsumers(context);
            ep.UseConsumeMessageFilter();
        }

        /// <summary>
        /// Registers a <see cref="ConsumeAccountMessageFilter{T}" /> to extract current account information from messages
        /// </summary>
        /// <param name="configurator"></param>
        private static void UseConsumeMessageFilter(this IConsumePipeConfigurator configurator)
        {
            configurator.ConnectConsumerConfigurationObserver(new ConsumeMessageConfigurationObserver());
        }

        /// <summary>
        /// Works just as <see cref="MessageSchedulerExtensions.AddMessageScheduler"/>, but uses
        /// <see cref="ISendEndpointProvider"/> instead of <see cref="IBus" /> to resolve the <see cref="ISendEndpoint"/>.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="schedulerEndpointAddress"></param>
        private static void AddScopedMessageScheduler(this IRegistrationConfigurator configurator, Uri schedulerEndpointAddress)
        {
            configurator.AddMessageScheduler(new ScopedEndpointMessageSchedulerRegistration(schedulerEndpointAddress));
        }

        private class ScopedEndpointMessageSchedulerRegistration : IMessageSchedulerRegistration
        {
            private readonly Uri _schedulerEndpointAddress;

            public ScopedEndpointMessageSchedulerRegistration(Uri schedulerEndpointAddress)
            {
                _schedulerEndpointAddress = schedulerEndpointAddress;
            }

            public void Register(IContainerRegistrar registrar)
            {
                registrar.Register(provider => CreateMessageScheduler(
                    provider.GetRequiredService<ISendEndpointProvider>(), provider.GetRequiredService<IBus>()));
            }

            private IMessageScheduler CreateMessageScheduler(ISendEndpointProvider sendEndpointProvider, IBus bus)
            {
                Task<ISendEndpoint> GetSchedulerEndpoint()
                {
                    return sendEndpointProvider.GetSendEndpoint(_schedulerEndpointAddress);
                }

                return new MessageScheduler(new EndpointScheduleMessageProvider(GetSchedulerEndpoint), bus.Topology);
            }
        }
    }
}
