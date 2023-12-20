using Microsoft.EntityFrameworkCore;

namespace Unflow.Cli;

public class GlobalContextFactory : IDbContextFactory<GlobalDbContext>
{
    public GlobalDbContext CreateDbContext()
        => new();
}