using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Digests;
using Unflow;
using Unflow.Cli;

public class GroupNameDownloader
{
    private readonly IDbContextFactory<GlobalDbContext> _globalDbContextFactory;
    private IRemoteNntpClient _client;

    public GroupNameDownloader(IDbContextFactory<GlobalDbContext> globalDbContextFactory, IRemoteNntpClient client)
    {
        _globalDbContextFactory = globalDbContextFactory;
        _client = client;
    }
    
    public async Task DownloadGroupNames()
    {
        
        using var globalDbContext = _globalDbContextFactory.CreateDbContext();
        foreach (var remoteGroup in _client.GetGroups())
        {
            var group = await globalDbContext.Group.FirstOrDefaultAsync(x => x.Name == remoteGroup.Name);
            if (group == null)
            {
                group = new()
                {
                    Id = SecureGuidGenerator.Default.NewGuid(), 
                    Name = remoteGroup.Name,
                };
                globalDbContext.Group.Add(group);
            }
            
            // other update stuff
        }

        await globalDbContext.SaveChangesAsync();
    }
}