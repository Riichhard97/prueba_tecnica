using System;
using System.Collections.Generic;
using System.Linq;

namespace Nexu.Shared.Infrastructure.Queries
{
    public class ListResult<T>
    {
        public virtual List<T> Items { get; set; }

        public object Metadata { get; set; }

        public ListResult()
        {
        }

        public ListResult(IEnumerable<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Items = items as List<T> ?? items.ToList();
        }
    }

    public static class ListResult
    {
        public static ListResult<T> From<T>(IList<T> items, object metadata = null)
        {
            return new ListResult<T>(items)
            {
                Metadata = metadata
            };
        }
    }
}
