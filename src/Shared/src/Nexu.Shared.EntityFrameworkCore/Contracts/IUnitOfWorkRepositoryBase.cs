using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Nexu.Shared.EntityFrameworkCore.Contracts
{
    public interface IUnitOfWorkRepositoryBase <T> 
    {
        Task<IReadOnlyList<T>> GetAllAsync();

        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);

        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeString = null, bool disableTracking = true);

        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicate = null,
          Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
          List<Expression<Func<T, object>>> includes = null,
          bool disableTracking = true);
          
        Task<T> AddAsync (T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        void AddEntity(T entity);
        void UpdateEntity(T entity);
        void DeleteEntity(T entity);
        void UpdateRange(IEnumerable<T> entities);
    }
}
