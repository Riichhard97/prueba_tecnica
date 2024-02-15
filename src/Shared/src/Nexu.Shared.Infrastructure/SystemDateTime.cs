using System;

namespace Nexu.Shared.Infrastructure
{
    public sealed class SystemDateTime : IDateTime
    {
        public DateTime UtcNow { get { return DateTime.UtcNow; } }
    }
}
