using Microsoft.EntityFrameworkCore;
using System;
using Moq;
using Nexu.Core.Persistence;
using Nexu.Shared.EntityFrameworkCore;
using Nexu.Shared.Infrastructure;

namespace Nexu.Core.Test.Mocks
{
    public static class MockCoreDbContext
    {        
        public static Mock<UnitOfWork<CoreDbContext>> GetUnitOfWork()
        {
           var context = GetDataBaseConfigMemory();
           context.Database.EnsureDeleted();            
           var mockUnitOfWork = new Mock<UnitOfWork<CoreDbContext>>(context);            

           return mockUnitOfWork;
        }

        public static CoreDbContext GetSingleRepository() 
        {
            var context = GetDataBaseConfigMemory();

            return context;
        }

        private static CoreDbContext GetDataBaseConfigMemory()
        {
            var userCurrentAccessor = new Mock<ICurrentUserAccessor>();
            var clock = new Mock<IDateTime>();

            var options = new DbContextOptionsBuilder<CoreDbContext>()
                .UseInMemoryDatabase(databaseName: $"CoreDbContext-{Guid.NewGuid()}")
                .Options;

            var coreDbContextFake = new CoreDbContext(options, clock.Object,
                userCurrentAccessor.Object);
            coreDbContextFake.Database.EnsureDeleted();

            return coreDbContextFake;
        }
    }
}
