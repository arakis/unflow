using Microsoft.EntityFrameworkCore;

namespace Unflow;

public class GroupContextFactoryLocator
{
    public IDbContextFactory<GroupDbContext> GetGroupDbContextFactory(Group group)
        => new GroupContextFactory(group);
}