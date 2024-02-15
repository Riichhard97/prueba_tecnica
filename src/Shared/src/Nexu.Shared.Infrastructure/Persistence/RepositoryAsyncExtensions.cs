using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Nexu.Shared.Exceptions;

namespace Nexu.Shared.Infrastructure.Persistence
{
    public static class RepositoryAsyncExtensions
    {
        /// <summary>
        /// Finds an entity of type <typeparamref name="T"/> using the supplied <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <param name="id">The entity identifier.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A task containing the found entity.</returns>
        /// <exception cref="EntityNotFoundException">If result is <c>null</c>.</exception>
        public static async Task<T> FindAsync<T>(this IRepositoryBase repository, Guid id, CancellationToken cancellationToken = default)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var entity = await repository.GetAsync<T>(id, cancellationToken).ConfigureAwait(false);
            if (entity is null)
            {
                throw EntityNotFoundException.For<T>(id);
            }

            return entity;
        }

        public static Task<T> GetAsync<T>(this IRepositoryBase repository, Guid id, CancellationToken cancellationToken = default)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            return repository.GetAsync<T, Guid>(id, cancellationToken);
        }

        public static Task<T> FirstAsync<T>(this IRepositoryBase repository, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var query = repository.Query(null, includes);
            return repository.FirstAsync(query);
        }

        public static Task<T> FirstAsync<T>(this IRepositoryBase repository, Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (includes is null)
            {
                throw new ArgumentNullException(nameof(includes));
            }

            var query = repository.Query(condition, includes);
            return repository.FirstAsync(query);
        }

        public static Task<T> SingleAsync<T>(this IRepositoryBase repository, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var query = repository.Query(null, includes);
            return repository.SingleAsync(query);
        }

        public static Task<T> SingleAsync<T>(this IRepositoryBase repository, Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            return repository.SingleAsync(condition, ignoreFilters: false, includes);
        }

        public static Task<T> SingleAsync<T>(this IRepositoryBase repository, Expression<Func<T, bool>> condition, bool ignoreFilters, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (includes is null)
            {
                throw new ArgumentNullException(nameof(includes));
            }

            var query = repository.Query(condition, ignoreFilters, includes);
            return repository.SingleAsync(query);
        }

        public static Task<bool> AnyAsync<T>(this IRepositoryBase repository, CancellationToken cancellationToken = default)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var query = repository.Query<T>();
            return repository.AnyAsync(query, cancellationToken);
        }

        public static Task<bool> AnyAsync<T>(this IRepositoryBase repository, Expression<Func<T, bool>> condition, CancellationToken cancellationToken = default)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var query = repository.Query(condition);
            return repository.AnyAsync(query, cancellationToken);
        }

        public static Task<bool> AnyAsync<T>(this IRepositoryBase repository, Expression<Func<T, bool>> condition, bool ignoreFilters, CancellationToken cancellationToken = default)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var query = repository.Query(condition, ignoreFilters);
            return repository.AnyAsync(query, cancellationToken);
        }

        public static Task<bool> AllAsync<T>(this IRepositoryBase repository, Expression<Func<T, bool>> condition, CancellationToken cancellationToken = default)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var query = repository.Query<T>();
            return repository.AllAsync(query, condition, cancellationToken);
        }

        public static Task<int> CountAsync<T>(this IRepositoryBase repository)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var query = repository.Query<T>();
            return repository.CountAsync(query);
        }

        public static Task<int> CountAsync<T>(this IRepositoryBase repository, Expression<Func<T, bool>> condition, CancellationToken cancellationToken = default)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var query = repository.Query(condition);
            return repository.CountAsync(query, cancellationToken);
        }

        public static Task<List<T>> ListAsync<T>(this IRepositoryBase repository, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (includes is null)
            {
                throw new ArgumentNullException(nameof(includes));
            }

            var query = repository.Query(null, includes);
            return repository.ListAsync(query);
        }

        public static Task<List<T>> ListAsync<T>(this IRepositoryBase repository, Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (includes is null)
            {
                throw new ArgumentNullException(nameof(includes));
            }

            var query = repository.Query(condition, includes);
            return repository.ListAsync(query);
        }
    }
}
