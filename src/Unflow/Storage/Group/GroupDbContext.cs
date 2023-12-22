using Microsoft.EntityFrameworkCore;

namespace Unflow;

public class GroupDbContext : DbContext
{
    private readonly Group _group;

    public GroupDbContext(Group group)
    {
        _group = group;
    }

    public GroupDbContext(DbContextOptions<GlobalDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dataFile = Path.Combine(DirHelper.GetGroupDataDir(_group), "group.db");
        optionsBuilder.UseSqlite($"Data Source={dataFile}",
            b => b.MigrationsAssembly("Unflow.GroupDbMigrations"));
    }

    public DbSet<ArticleHeader> ArticleHeader { get; set; }
}