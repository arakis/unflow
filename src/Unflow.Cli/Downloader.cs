using Usenet.Nntp;
using Usenet.Nntp.Models;

namespace Unflow.Cli;

public class Downloader
{
    private NntpConnection _connection;
    private NntpClient _client;

    public Downloader()
    {
        _connection = new NntpConnection();
        _client = new NntpClient(_connection);
    }

    public async Task Connect()
    {
        await _client.ConnectAsync("news.eternal-september.org", 119, false);
    }

    public async Task DownloadGroup(GroupEntity group)
    {
        var nntpGroup = _client.Group(group.GroupName);

        var low = nntpGroup.Group.LowWaterMark;
        var high = nntpGroup.Group.HighWaterMark;
        low = Math.Max(group.LastDownloadedArticleNumber, low);

        if (high - low > 1000)
            high = low + 1000;

        var response = _client.Xover(new NntpArticleRange(low, high));
        foreach (var item in response.Lines.Select(x => new XoverItemResponse(x)).OrderBy(x => x.ArticleNumber).ToArray())
        {
            var article = _client.Article(item.MessageId);
            if (article.Success)
                await ProcessArticle(article.Article);
            else
                await ArticleError(item.MessageId);

            group.LastDownloadedArticleNumber = item.ArticleNumber;
        }
    }

    private async Task ProcessArticle(NntpArticle article)
    {
    }

    private async Task ArticleError(string messageId)
    {
    }
}