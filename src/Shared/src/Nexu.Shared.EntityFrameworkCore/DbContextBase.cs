using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Nexu.Shared.Infrastructure;
using Nexu.Shared.Model;

namespace Nexu.Shared.EntityFrameworkCore
{
    public abstract class DbContextBase : DbContext
    {
        private readonly ICurrentUserAccessor _currentUserAccessor;

        protected IDateTime Clock { get; }

        protected Guid? CurrentUserId
        {
            get { return _currentUserAccessor.TryGet(); }
        }

        protected DbContextBase(
            DbContextOptions options,
            IDateTime clock,
            ICurrentUserAccessor currentUserAccessor
        ) : base(options)
        {
            Clock = clock;
            _currentUserAccessor = currentUserAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var type = entity.ClrType;
                if (typeof(IEntity).IsAssignableFrom(type))
                {
                    modelBuilder.Entity(type).Property(nameof(IEntity.Id)).HasValueGenerator<NewIdValueGenerator>();
                }
                if (typeof(IHaveDateCreated).IsAssignableFrom(type))
                {
                    modelBuilder.Entity(type).HasIndex(nameof(IHaveDateCreated.DateCreated));
                }

                if (typeof(IHaveDateUpdated).IsAssignableFrom(type))
                {
                    modelBuilder.Entity(type).HasIndex(nameof(IHaveDateUpdated.DateUpdated));
                }

                if (typeof(IHaveDateDeleted).IsAssignableFrom(type))
                {
                    modelBuilder.Entity(type).HasIndex(nameof(IHaveDateDeleted.DateDeleted));
                }

                if (type.GetInterfaces().Any(i => IsClosedTypeOf(i, typeof(IStatus<>))))
                {
                    modelBuilder.Entity(type).HasIndex(nameof(IStatus<string>.Status));
                }

                if (typeof(ICreatedBy).IsAssignableFrom(type))
                {
                    modelBuilder.Entity(type).HasIndex(nameof(ICreatedBy.CreatedById));
                }

                if (typeof(IUpdatedBy).IsAssignableFrom(type))
                {
                    modelBuilder.Entity(type).HasIndex(nameof(IUpdatedBy.UpdatedById));
                }

                if (typeof(IDeletedBy).IsAssignableFrom(type))
                {
                    modelBuilder.Entity(type).HasIndex(nameof(IDeletedBy.DeletedById));
                }

                if (typeof(ISoftDelete).IsAssignableFrom(type))
                {
                    modelBuilder.Entity(type).HasIndex(nameof(ISoftDelete.IsDeleted));
                }

                ConfigureEntity(modelBuilder, entity);

                foreach (var foreignKey in entity.GetForeignKeys())
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }
        }

        protected virtual void ConfigureEntity(ModelBuilder modelBuilder, IMutableEntityType entity)
        {
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            foreach (var entityEntry in ChangeTracker.Entries())
            {
                if (entityEntry.State == EntityState.Added)
                {
                    if (entityEntry.Entity is IHaveDateCreated haveDateCreated)
                    {
                        haveDateCreated.DateCreated = Clock.UtcNow;
                    }
                    if (CurrentUserId != null)
                    {
                        if (entityEntry.Entity is ICreatedBy createdBy && CurrentUserId != default)
                        {
                            createdBy.SetCreatedBy(CurrentUserId.Value);
                        }
                    }

                    OnBeforeAdd(entityEntry);
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    if (entityEntry.Entity is IHaveDateUpdated haveDateUpdated)
                    {
                        haveDateUpdated.DateUpdated = Clock.UtcNow;
                    }

                    if (CurrentUserId != null)
                    {
                        if (entityEntry.Entity is IUpdatedBy updatedBy && CurrentUserId != Guid.Empty)
                        {
                            updatedBy.SetUpdatedById(CurrentUserId.Value);
                        }
                    }

                    OnBeforeUpdate(entityEntry);
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected virtual void OnBeforeAdd(EntityEntry entityEntry)
        {
        }

        protected virtual void OnBeforeUpdate(EntityEntry entityEntry)
        {
        }

        protected static bool IsClosedTypeOf(Type type, Type genericInterfaceType)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == genericInterfaceType;
        }
    }
}
