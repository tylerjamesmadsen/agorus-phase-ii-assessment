using Microsoft.EntityFrameworkCore;

namespace Agorus.Data
{

    public class AgorusContext : DbContext
    {
        public DbSet<Models.File> Files { get; set; }

        public string DbPath { get; }

        public AgorusContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "agorus.data.filestorage.db");
        }

        internal AgorusContext(DbContextOptions options) : base(options)
        {
            DbPath = string.Empty;
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured) options.UseSqlite($"Data Source={DbPath}");
        }
    }
}
