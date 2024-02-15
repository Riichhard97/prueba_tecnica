using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nexu.Shared.Common;

namespace Nexu.Shared.EntityFrameworkCore.Contracts    
{
    public interface IUnitOfWork : IDisposable
    {        
        IUnitOfWorkRepositoryBase<TEntity> Repository <TEntity>() where TEntity : BaseDomain;
        Task<int> Complete();
    }
}
