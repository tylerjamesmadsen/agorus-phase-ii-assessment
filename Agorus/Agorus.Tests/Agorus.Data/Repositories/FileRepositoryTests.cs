using Agorus.Data.Repositories;
using FluentAssertions;

namespace Agorus.Tests.Agorus.Data.Repositories
{
    [TestClass]
    public class FileRepositoryTests
    {
        [TestMethod]
        public async Task GetAllAsync_WhenNoFiles_ShouldReturnEmpty()
        {
            // Arrange
            using var context = DataTestUtilities.GetTestContext();
            await context.Database.EnsureCreatedAsync();
            var repo = new FileRepository(context);

            // Act
            var result = await repo.GetAllAsync(default);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
