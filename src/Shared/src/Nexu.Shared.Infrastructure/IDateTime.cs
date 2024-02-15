using System;

namespace Nexu.Shared.Infrastructure
{
    /// <summary>
    /// Represents a clock abstraction
    /// </summary>
    public interface IDateTime
    {
        DateTime UtcNow { get; }
    }
}
