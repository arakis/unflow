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
        optionsBuilder.UseSqlite("Data Source=global.db",
            b => b.MigrationsAssembly("Unflow.DbMigrations"));
    }

    public DbSet<Group> Group { get; set; }
}