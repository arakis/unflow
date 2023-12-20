using Microsoft.EntityFrameworkCore;

namespace Unflow;

public class GroupContextFactory : IDbContextFactory<GroupDbContext>
{
    private readonly Group _group;

    public GroupContextFactory(Group group)
    {
        _group = group;
    }
    
    public GroupDbContext CreateDbContext()
        => new(_group);
}