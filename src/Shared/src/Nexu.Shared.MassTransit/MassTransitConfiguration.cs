using System;

namespace Nexu.Shared.MassTransit
{
    public class MassTransitConfiguration
    {
        public Uri RabbitMQ { get; set; }

        public string ActiveMQ { get; set; }

        public string AzureServiceBus { get; set; }

        public string Failover { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool UseInMemory { get; set; }
    }

    public static class MassTransitConfigurationExtensions
    {
        public static string[] GetFailoverHosts(this MassTransitConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var hosts = configuration.Failover?.Split(",");

            if (hosts?.Length > 0)
            {
                // TODO: Validate host names?
                //foreach (var host in hosts)
                //{
                //    if (!Uri.IsWellFormedUriString(host, UriKind.Absolute))
                //    {
                //        throw new AppConfigurationException($"Failover host {host} is not a well formed URL.");
                //    }
                //}

                return hosts;
            }

            return null;
        }
    }
}
