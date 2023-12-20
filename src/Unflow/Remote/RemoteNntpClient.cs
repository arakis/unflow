using Unflow.Cli;
using Usenet.Nntp;
using Usenet.Nntp.Models;

public class RemoteNntpClient : IRemoteNntpClient
{
    private NntpClient _client;
    private INntpConnection _connection;

    public RemoteNntpClient()
    {
        _connection = new NntpConnection();
        _client = new NntpClient(_connection);
    }

    public Task<bool> ConnectAsync(string hostname, int port, bool useSsl)
        => _client.ConnectAsync(hostname, port, useSsl);

    public RemoteGroupInfo GetGroupInfo(string name)
    {
        var nativeRemoteGroup = _client.Group(name);
        return new RemoteGroupInfo
        {
            Name = nativeRemoteGroup.Group.Name,
            LowestAvailableArticleNumber = nativeRemoteGroup.Group.LowWaterMark,
            HighestAvailableArticleNumber = nativeRemoteGroup.Group.HighWaterMark
        };
    }

    public RemotePartialHeader[] GetPartialHeaderForGroup(string name, ArticleRange range)
    {
        var response = _client.Xover(new NntpArticleRange(range.Start, range.End));
        return response.Lines.Select(x => new RemotePartialHeader(x)).OrderBy(x => x.ArticleNumber).ToArray();
    }
}