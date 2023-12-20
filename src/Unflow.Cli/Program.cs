using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Usenet;
using Usenet.Nntp.Parsers;

namespace Unflow.Cli;

class Program
{
    private static IDbContextFactory<GlobalDbContext> _factory;
    private static GroupContextFactoryLocator _groupContextFactoryLocator;

    static async Task Main(string[] args)
    {
        _factory = new GlobalContextFactory();
        _groupContextFactoryLocator = new GroupContextFactoryLocator();

        await MigrateDatabase();
        await DownloadGroupNames();
        await DownloadPartialHeaders();
    }

    private static async Task MigrateDatabase()
    {
        using var context = _factory.CreateDbContext();
        await context.Database.MigrateAsync();
        Parallel.ForEach(context.Group, async group =>
        {
            var groupContext = await _groupContextFactoryLocator.GetGroupDbContextFactory(group).CreateDbContextAsync();
            await groupContext.Database.MigrateAsync();
        });
    }

    private async static Task DownloadGroupNames()
    {
        using IRemoteNntpClient client = await CreateClient();
        var downloader = new GroupNameDownloader(_factory, client);
        await downloader.DownloadGroupNames();
    }

    private async static Task DownloadPartialHeaders()
    {
        using IRemoteNntpClient client = await CreateClient();
        var downloader = new PartialHeaderDownloader(_groupContextFactoryLocator, client);

        using var context = _factory.CreateDbContext();
        foreach (var group in context.Group)
        {
            var remoteGroupInfo = client.GetGroupInfo(group.Name);
            await downloader.DownloadPartialHeaders(remoteGroupInfo, group);
        }
    }

    private static async Task<IRemoteNntpClient> CreateClient()
    {
        IRemoteNntpClient client = new RemoteNntpClient();
        await client.ConnectAsync("news.eternal-september.org", 119, false);
        return client;
    }
}