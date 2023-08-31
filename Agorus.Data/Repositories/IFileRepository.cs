namespace Agorus.Data.Repositories
{
    public interface IFileRepository
    {
        Task<Models.File?> AddAsync(Models.File file, CancellationToken cancellationToken);
        Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> DeleteByFileIdAndVersionAsync(Guid fileId, int version, CancellationToken cancellationToken);
        Task<IEnumerable<Models.File>> GetAllAsync(Guid fileId, CancellationToken cancellationToken);
        Task<Models.File?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Models.File?> GetByFileIdAndVersionAsync(Guid fileId, int version, CancellationToken cancellationToken);
        Task<Models.File?> UpdateAsync(Models.File file, CancellationToken cancellationToken);
    }
}