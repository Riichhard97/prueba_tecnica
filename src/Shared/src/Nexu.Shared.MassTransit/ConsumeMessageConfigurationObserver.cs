using System;
using MassTransit.ConsumeConfigurators;

namespace Nexu.Shared.MassTransit
{
    public sealed class ConsumeMessageConfigurationObserver : IConsumerConfigurationObserver
    {
        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator) where TConsumer : class
        {
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            var accountFilter = new ConsumeAccountMessagePipeSpecification<TMessage>();
            var loggingFilter = new LoggingScopeFilterSpecification<TMessage>();
            configurator.Message(m => m.AddPipeSpecification(accountFilter));
            configurator.Message(m => m.AddPipeSpecification(loggingFilter));
        }
    }
}
