using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Unflow.DbMigrations;

public class GroupDesignTimeDbContextFactory : IDesignTimeDbContextFactory<GroupDbContext>
{
    public GroupDbContext CreateDbContext(string[] args)
        => new(new Group() { Name = ".dummy" });
}