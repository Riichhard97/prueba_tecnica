using System;
using System.Diagnostics.CodeAnalysis;
using MassTransit;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nexu.Shared.EntityFrameworkCore
{
    public sealed class NewIdValueGenerator : Microsoft.EntityFrameworkCore.ValueGeneration.ValueGenerator<Guid>
    {
        public static readonly NewIdValueGenerator Instance = new();

        public override bool GeneratesTemporaryValues => false;

        public override Guid Next([NotNull] EntityEntry entry)
        {
            return NewId.NextGuid();
        }
    }
}
