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
    static async Task Main(string[] args)
    {
        _factory = new GlobalContextFactory();
        await MigrateDatabase();
        await DownloadGroupNames();
    }

    private static async Task MigrateDatabase()
    {
        using var context = _factory.CreateDbContext();
        await context.Database.MigrateAsync();
    }

    private async static Task DownloadGroupNames()
    {
        var client = await CreateClient();
        var downloader = new GroupNameDownloader(_factory, client);
        await downloader.DownloadGroupNames();
    }

    private async static Task DownloadPartialHeaders()
    {
        var client = await CreateClient();
        var downloader = new PartialHeaderDownloader(client);

        var localGroupInfo = new Group { Name = "eternal-september.test" };

        var remoteGroupInfo = client.GetGroupInfo(localGroupInfo.Name);
        downloader.DownloadPartialHeaders(remoteGroupInfo, localGroupInfo);
    }

    private static async Task<IRemoteNntpClient> CreateClient()
    {
        IRemoteNntpClient client = new RemoteNntpClient();
        await client.ConnectAsync("news.eternal-september.org", 119, false);
        return client;
    }
}