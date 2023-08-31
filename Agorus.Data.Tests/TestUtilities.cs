using Microsoft.EntityFrameworkCore;

namespace Agorus.Data.Tests
{
    internal static class TestUtilities
    {
        internal static async Task<AgorusContext> GetTestContextAsync()
        {
            var options = new DbContextOptionsBuilder<AgorusContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new AgorusContext(options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        internal static async Task<AgorusContext> GetTestContextAsync(IEnumerable<object> data)
        {
            var context = await GetTestContextAsync();
            if (data.Any())
            {
                await context.AddRangeAsync(data);
                await context.SaveChangesAsync();
            }

            return context;
        }

        internal static async Task<AgorusContext> GetTestContextAsync(params object[] data)
        {
            var context = await GetTestContextAsync();
            if (data.Any() == true)
            {
                foreach (var item in data)
                {
                    await context.AddAsync(item);
                }

                await context.SaveChangesAsync();
            }

            return context;
        }
    }
}
