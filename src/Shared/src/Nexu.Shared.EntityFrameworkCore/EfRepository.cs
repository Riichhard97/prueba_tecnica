using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Nexu.Shared.Infrastructure.Persistence;

namespace Nexu.Shared.EntityFrameworkCore
{
    public class EfRepository : EfRepositoryBase, IRepository
    {
        public EfRepository(DbContext context)
            : base(context, false)
        {
        }

        public IReadOnlyRepository AsReadOnly()
        {
            return new ReadOnlyEfRepository(Context);
        }

        public virtual T Add<T>(T entity)
            where T : class
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = Set<T>().Add(entity);

            return result.Entity;
        }

        public virtual T Remove<T>(T entity)
            where T : class
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = Set<T>().Remove(entity);

            return result.Entity;
        }

        public virtual T Update<T>(T entity)
            where T : class
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var result = Set<T>().Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            return result.Entity;
        }

        public virtual Task<int> SaveChangesAsync(CancellationToken token = default)
        {
            return Context.SaveChangesAsync(token);
        }

        public Task<IRepositoryTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Context.Database.BeginTransactionAsync(cancellationToken)
                .ContinueWith<IRepositoryTransaction>(task => new RepositoryTransactionImpl(task.Result));
        }

        private class RepositoryTransactionImpl : IRepositoryTransaction
        {
            private readonly IDbContextTransaction _transaction;

            public RepositoryTransactionImpl(IDbContextTransaction transaction)
            {
                _transaction = transaction;
            }

            public Task CommitAsync(CancellationToken cancellationToken = default)
            {
                return _transaction.CommitAsync(cancellationToken);
            }

            public void Dispose()
            {
                _transaction.Dispose();
            }

            public ValueTask DisposeAsync()
            {
                return _transaction.DisposeAsync();
            }

            public Task RollbackAsync(CancellationToken cancellationToken = default)
            {
                return _transaction.RollbackAsync(cancellationToken);
            }
        }
    }

    public class EfRepository<TContext> : EfRepository
        where TContext : DbContext
    {
        public EfRepository(TContext context)
            : base(context)
        {
        }
    }
}
