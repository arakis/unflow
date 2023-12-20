using Microsoft.EntityFrameworkCore;

namespace Unflow;

public class GlobalDbContext : DbContext
{
    public GlobalDbContext()
    {
    }

    public GlobalDbContext(DbContextOptions<GlobalDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dataFile = Path.Combine(DirHelper.RootDataDir, "global.db");

        optionsBuilder.UseSqlite($"Data Source={dataFile}",
            b => b.MigrationsAssembly("Unflow.GlobalDbMigrations"));
    }

    public DbSet<Group> Group { get; set; }
}