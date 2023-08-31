using Agorus.Data.Repositories;
using FluentAssertions;
using File = Agorus.Data.Models.File;

namespace Agorus.Data.Tests.Repositories
{
    [TestClass]
    public class FileRepositoryTests
    {
        [TestMethod]
        public async Task GetAllAsync_WhenNoFiles_ShouldReturnEmpty()
        {
            // Arrange
            using var context = await TestUtilities.GetTestContextAsync();
            var repo = new FileRepository(context);

            // Act
            var result = await repo.GetAllAsync(default);

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllFiles()
        {
            // Arrange
            var files = new[]
            {
                new File { Id = 1 },
                new File { Id = 2 },
            };
            using var context = await TestUtilities.GetTestContextAsync(files);

            var repo = new FileRepository(context);

            // Act
            var result = await repo.GetAllAsync(default);

            // Assert
            result.Should().BeEquivalentTo(files);
        }

        [TestMethod]
        public async Task GetAllAsync_GivenFileId_ShouldReturnAllFilesWithMatchingFileId()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var files = new[]
            {
                new File { Id = 1, FileId = fileId },
                new File { Id = 2, FileId = fileId },
                new File { Id = 3, FileId = Guid.NewGuid() },
            };
            using var context = await TestUtilities.GetTestContextAsync(files);

            var expected = files[..2];

            var repo = new FileRepository(context);

            // Act
            var result = await repo.GetAllAsync(fileId, default);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAsync_WhenNoMatchingIdFound_ShouldReturnNull()
        {
            // Arrange
            var files = new[]
            {
                new File { Id = 1 },
                new File { Id = 2 },
            };
            using var context = await TestUtilities.GetTestContextAsync(files);

            var repo = new FileRepository(context);

            // Act
            var result = await repo.GetAsync(99, default);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAsync_ShouldReturnFileWithMatchingId()
        {
            // Arrange
            var id = 1;
            var files = new[]
            {
                new File { Id = id },
                new File { Id = 2 },
            };
            using var context = await TestUtilities.GetTestContextAsync(files);

            var repo = new FileRepository(context);

            // Act
            var result = await repo.GetAsync(id, default);

            // Assert
            result.Should().Be(files[0]);
        }

        [TestMethod]
        public async Task GetLatestVersionAsync_ShouldReturnTheLatestVersionForTheSpecifiedFileId()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var files = new[]
            {
                new File { FileId = fileId, Version = 1 },
                new File { FileId = fileId, Version = 2 },
                new File { FileId = fileId, Version = 3 },
            };
            using var context = await TestUtilities.GetTestContextAsync(files);

            var repo = new FileRepository(context);

            // Act
            var result = await repo.GetLatestVersionAsync(fileId, default);

            // Assert
            result.Should().Be(files[2]);
        }

        [TestMethod]
        public async Task GetAsync_WhenFileIdOrVersionNotFound_ShouldReturnNull()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var version = 99;
            var files = new[]
            {
                new File { FileId = fileId, Version = 1 },
                new File { FileId = fileId, Version = 2 },
                new File { FileId = fileId, Version = 3 },
            };
            using var context = await TestUtilities.GetTestContextAsync(files);

            var repo = new FileRepository(context);

            // Act
            var result = await repo.GetAsync(fileId, version, default);

            // Assert
            result.Should().BeNull();
        }
        
        [TestMethod]
        public async Task GetAsync_ShouldReturnFileWithMatchingFileIdAndVersion()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var version = 2;
            var files = new[]
            {
                new File { FileId = fileId, Version = 1 },
                new File { FileId = fileId, Version = version },
                new File { FileId = fileId, Version = 3 },
            };
            using var context = await TestUtilities.GetTestContextAsync(files);

            var repo = new FileRepository(context);

            // Act
            var result = await repo.GetAsync(fileId, version, default);

            // Assert
            result.Should().Be(files[1]);
        }
        
        [TestMethod]
        public async Task AddAsync_ShouldSetFileIdAndVersionAndReturnCreatedFile()
        {
            // Arrange
            var file = new File
            {
                Content = Array.Empty<byte>(),
                ContentType = "test-content-type",
            };
            using var context = await TestUtilities.GetTestContextAsync();

            var repo = new FileRepository(context);

            // Act
            var result = await repo.AddAsync(file, default);

            // Assert
            result.Should().NotBeNull();
            result!.FileId.Should().NotBeEmpty();
            result!.Version.Should().Be(1);
        }
        
        [TestMethod]
        public async Task UpdateAsync_WhenFileNotFound_ShouldReturnNull()
        {
            // Arrange
            var file = new File
            {
                FileId = Guid.NewGuid(),
                Content = Array.Empty<byte>(),
                ContentType = "test-content-type",
            };
            using var context = await TestUtilities.GetTestContextAsync();

            var repo = new FileRepository(context);

            // Act
            var result = await repo.UpdateAsync(file, default);

            // Assert
            result.Should().BeNull();
        }
        
        [TestMethod]
        public async Task UpdateAsync_ShouldCreateNewVersionWithSameFileIdAndReturnUpdatedFile()
        {
            // Arrange
            var v1 = new File
            {
                Version = 1,
                Content = Array.Empty<byte>(),
                ContentType = "test-content-type",
            };
            using var context = await TestUtilities.GetTestContextAsync(v1);
            var v2 = new File
            {
                FileId = v1.FileId,
                Content = new[] { (byte)1, (byte)2, (byte)3 },
            };

            var expected = new File
            {
                Id = 2,
                Content = v2.Content,
                ContentType = v2.ContentType,
                FileId = v1.FileId,
                Version = 2,
            };

            var repo = new FileRepository(context);

            // Act
            var result = await repo.UpdateAsync(v2, default);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenIdNotFound_ShouldReturnFalse()
        {
            // Arrange
            using var context = await TestUtilities.GetTestContextAsync();
            var repo = new FileRepository(context);

            // Act
            var result = await repo.DeleteAsync(1, default);

            // Assert
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public async Task DeleteAsync_WhenIdFoundAndFileRemovedFromDatabase_ShouldReturnTrue()
        {
            // Arrange
            var file = new File();
            using var context = await TestUtilities.GetTestContextAsync(file);
            var repo = new FileRepository(context);

            // Act
            var result = await repo.DeleteAsync(1, default);

            // Assert
            result.Should().BeTrue();
        }
        
        [TestMethod]
        public async Task DeleteAsync_WhenFileIdAndVersionNotFound_ShouldReturnFalse()
        {
            // Arrange
            var file = new File
            {
                FileId = Guid.NewGuid(),
                Version = 1,
            };
            using var context = await TestUtilities.GetTestContextAsync(file);
            var repo = new FileRepository(context);

            // Act
            var result = await repo.DeleteAsync(file.FileId, 2, default);

            // Assert
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public async Task DeleteAsync_WhenFileIdAndVersionFoundAndFileRemovedFromDatabase_ShouldReturnTrue()
        {
            // Arrange
            var file = new File
            {
                FileId = Guid.NewGuid(),
                Version = 1,
            };
            using var context = await TestUtilities.GetTestContextAsync(file);
            var repo = new FileRepository(context);

            // Act
            var result = await repo.DeleteAsync(file.FileId, file.Version, default);

            // Assert
            result.Should().BeTrue();
        }
    }
}
