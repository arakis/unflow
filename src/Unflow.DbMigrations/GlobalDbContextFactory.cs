using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Unflow.DbMigrations;

public class GlobalDbContextFactory : IDesignTimeDbContextFactory<GlobalDbContext>
{
    public GlobalDbContext CreateDbContext(string[] args)
        => new();
}