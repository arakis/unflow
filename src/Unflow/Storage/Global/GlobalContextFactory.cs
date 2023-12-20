using Microsoft.EntityFrameworkCore;

namespace Unflow;

public class GlobalContextFactory : IDbContextFactory<GlobalDbContext>
{
    public GlobalDbContext CreateDbContext()
        => new();
}