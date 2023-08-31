using Microsoft.EntityFrameworkCore;
using File = Agorus.Data.Models.File;

namespace Agorus.Data.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly AgorusContext _context;

        public FileRepository(AgorusContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<File>> GetAllAsync(Guid fileId, CancellationToken cancellationToken)
        {
            var query = _context.Files.AsQueryable();
            if (fileId != default)
            {
                query = query.Where(f => f.FileId == fileId);
            }
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<File?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Files
                .Where(f => f.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<File?> GetByFileIdAndVersionAsync(Guid fileId, int version, CancellationToken cancellationToken)
        {
            return await _context.Files
                .Where(f => f.FileId == fileId && f.Version == version)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public async Task<File?> GetLatestVersionAsync(Guid fileId, CancellationToken cancellationToken)
        {
            return await _context.Files
                .Where(f => f.FileId == fileId)
                .OrderByDescending(f => f.Version)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<File?> AddAsync(File file, CancellationToken cancellationToken)
        {
            file.FileId = Guid.NewGuid();
            file.Version = 1;
            await _context.Files.AddAsync(file, cancellationToken);
            var didCreate = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (didCreate)
            {
                return file;
            }

            return null;
        }

        public async Task<File?> UpdateAsync(File file, CancellationToken cancellationToken)
        {
            var latestVersion = await GetLatestVersionAsync(file.FileId, cancellationToken);
            if (latestVersion != null)
            {
                var newVersion = new File
                {
                    FileId = latestVersion.FileId,
                    Version = latestVersion.Version + 1,
                    FileName = file.FileName,
                    Content = file.Content,
                    ContentType = file.ContentType,
                };
                await _context.AddAsync(newVersion, cancellationToken);
                var didUpdate = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (didUpdate)
                {
                    return newVersion;
                }
            }

            return null;
        }

        public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var file = await GetByIdAsync(id, cancellationToken);
            if (file != null)
            {
                _context.Files.Remove(file);
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }

            return false;
        }

        public async Task<bool> DeleteByFileIdAndVersionAsync(Guid fileId, int version, CancellationToken cancellationToken)
        {
            var file = await GetByFileIdAndVersionAsync(fileId, version, cancellationToken);
            if (file != null)
            {
                _context.Files.Remove(file);
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }

            return false;
        }
    }
}
