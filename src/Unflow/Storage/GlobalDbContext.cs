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
        var userDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var unflowDataDir = Path.Combine(userDataDir, "unflow");
        Directory.CreateDirectory(unflowDataDir);
        var dataFile = Path.Combine(unflowDataDir, "global.db");

        optionsBuilder.UseSqlite($"Data Source={dataFile}",
            b => b.MigrationsAssembly("Unflow.DbMigrations"));
    }

    public DbSet<Group> Group { get; set; }
}