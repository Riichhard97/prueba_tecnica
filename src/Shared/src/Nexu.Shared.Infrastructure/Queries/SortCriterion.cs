using System.ComponentModel;
using System.Diagnostics;

namespace Nexu.Shared.Infrastructure.Queries
{
    [DebuggerDisplay("{Name,nq} {Direction}")]
    public sealed class SortCriterion
    {
        public string Name { get; }

        public ListSortDirection Direction { get; }

        public SortCriterion(string name, ListSortDirection direction)
        {
            Name = name;
            Direction = direction;
        }
    }
}
