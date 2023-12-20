using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Usenet;
using Usenet.Nntp.Parsers;

namespace Unflow.Cli;

class Program
{
    static async Task Main(string[] args)
    {
        IRemoteNntpClient client = new RemoteNntpClient();
        await client.ConnectAsync("news.eternal-september.org", 119, false);
        var downloader = new PartialHeaderDownloader(client);
        
        var localGroupInfo = new Group { Name = "eternal-september.test" };

        var remoteGroupInfo = client.GetGroupInfo(localGroupInfo.Name);
        downloader.DownloadPartialHeaders(remoteGroupInfo, localGroupInfo);
        downloader.DownloadPartialHeaders(remoteGroupInfo, localGroupInfo);
    }
}