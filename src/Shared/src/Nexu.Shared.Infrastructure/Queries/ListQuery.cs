using System.Collections.Generic;
using MediatR;

namespace Nexu.Shared.Infrastructure.Queries
{
    public class ListQuery
    {
        public string OrderBy { get; set; }

        public string Term { get; set; }
    }

    public class ListQuery<T> : ListQuery, IRequest<ListResult<T>>
    {
    }

    public static class ListQueryExtensions
    {
        public static ICollection<SortCriterion> GetSortCriteria(this ListQuery query)
        {
            return SortCriteriaHelper.GetSortCriteria(query?.OrderBy);
        }
    }
}
