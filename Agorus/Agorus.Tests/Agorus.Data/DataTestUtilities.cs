using Agorus.Data;
using Microsoft.EntityFrameworkCore;

namespace Agorus.Tests.Agorus.Data
{
    internal static class DataTestUtilities
    {
        internal static AgorusContext GetTestContext()
        {
            var options = new DbContextOptionsBuilder<AgorusContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AgorusContext(options);
        }
    }
}
