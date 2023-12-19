using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Usenet;
using Usenet.Nntp.Parsers;

namespace Unflow.Cli;

class Program
{
    static async Task Main(string[] args)
    {
        var downloader = new Downloader();
        await downloader.Connect();
        var group = new GroupEntity()
        {
            GroupName = "eternal-september.test",
            LastDownloadedArticleNumber = 0,
        };
        await downloader.DownloadGroup(group);
    }
}