using FluentValidation;

namespace Nexu.Shared.Infrastructure.Queries
{
    public abstract class PaginatedQueryValidator<T> : AbstractValidator<T>
        where T : PaginatedQuery
    {
        protected PaginatedQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(PaginatedQuery.MinPage);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(PaginatedQuery.MinPageSize)
                .LessThanOrEqualTo(PaginatedQuery.MaxPageSize);
        }
    }

    public sealed class PaginatedQueryValidator : PaginatedQueryValidator<PaginatedQuery>
    {
    }
}
