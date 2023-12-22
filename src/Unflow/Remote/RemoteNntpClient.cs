using Unflow.ArticleBlobStorage;
using Unflow.Cli;
using Usenet.Nntp;
using Usenet.Nntp.Builders;
using Usenet.Nntp.Models;
using Usenet.Nntp.Parsers;

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
        var response = _client.Group(name);
        return ConvertToRemoteGroupInfo(response.Group);
    }

    public RemotePartialHeader[] GetPartialHeaderForGroup(string name, ArticleRange range)
    {
        var response = _client.Xover(new NntpArticleRange(range.Start, range.End));
        return response.Lines.Select(x => new RemotePartialHeader(x)).OrderBy(x => x.ArticleNumber).ToArray();
    }

    public RemoteGroupInfo[] GetGroups()
    {
        var response = _client.ListActive();
        return response.Groups.Select(x => ConvertToRemoteGroupInfo(x)).ToArray();
    }

    public Article GetArticle(string messageId)
    {
        var response = _connection.MultiLineCommand<CustomNntpArticleResponse>(string.Format("ARTICLE {0}", messageId), (IMultiLineResponseParser<CustomNntpArticleResponse>)new CustomArticleResponseParser(null, CustomArticleRequestType.Article));
        // var response = _client.Article(messageId);
        return response.Article;
    }

    private static RemoteGroupInfo ConvertToRemoteGroupInfo(NntpGroup nativeRemoteGroup)
    {
        return new()
        {
            Name = nativeRemoteGroup.Name,
            LowestAvailableArticleNumber = nativeRemoteGroup.LowWaterMark,
            HighestAvailableArticleNumber = nativeRemoteGroup.HighWaterMark
        };
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null!;
    }
}