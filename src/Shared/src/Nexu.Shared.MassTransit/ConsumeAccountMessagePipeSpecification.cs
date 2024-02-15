using System;
using System.Collections.Generic;
using GreenPipes;
using MassTransit;

namespace Nexu.Shared.MassTransit
{
    public sealed class ConsumeAccountMessagePipeSpecification<T> : IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            var filter = new ConsumeAccountMessageFilter<T>();
            builder.AddFilter(filter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}
