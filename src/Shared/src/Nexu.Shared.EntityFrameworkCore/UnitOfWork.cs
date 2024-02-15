using Microsoft.EntityFrameworkCore;
using Nexu.Shared.EntityFrameworkCore.Contracts;
using Nexu.Shared.Common;
using System.Threading.Tasks;
using System.Collections;
using System;

namespace Nexu.Shared.EntityFrameworkCore
{
    public class UnitOfWork <TContext> : IUnitOfWork where TContext : DbContext
    {
        private Hashtable _repositorires;
        public DbContext Context;
        public UnitOfWork(TContext context) { Context = context; }        
        public async Task<int> Complete()
        {
            return await Context.SaveChangesAsync();
        }
        public void Dispose()
        {
            Context.Dispose();
        }
        public IUnitOfWorkRepositoryBase<TEntity> Repository<TEntity>() where TEntity : BaseDomain
        {
            if(_repositorires == null)
            {
                _repositorires = new Hashtable();
            }

            var type = typeof(TEntity).Name;

            if (!_repositorires.ContainsKey(type))
            {
                var respositoryType = typeof(UnitWorkRepositoryBase<>);
                var respositoryInstance = Activator
                    .CreateInstance(respositoryType.MakeGenericType(typeof(TEntity)), Context);
                _repositorires.Add(type, respositoryInstance);
            }

            return (IUnitOfWorkRepositoryBase<TEntity>)_repositorires[type];
        }
    }
}
