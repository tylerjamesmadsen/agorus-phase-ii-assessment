namespace Agorus.Data.Repositories
{
    public interface IFileRepository
    {
        Task<Models.File?> AddAsync(Models.File file, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid fileId, int version, CancellationToken cancellationToken);
        Task<IEnumerable<Models.File>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Models.File>> GetAllAsync(Guid fileId, CancellationToken cancellationToken);
        Task<Models.File?> GetAsync(int id, CancellationToken cancellationToken);
        Task<Models.File?> GetAsync(Guid fileId, int version, CancellationToken cancellationToken);
        Task<Models.File?> UpdateAsync(Models.File file, CancellationToken cancellationToken);
    }
}