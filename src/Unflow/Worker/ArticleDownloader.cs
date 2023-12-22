using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Digests;
using Unflow;
using Unflow.ArticleBlobStorage;
using Unflow.Cli;

public class ArticleDownloader
{
    private readonly GroupContextFactoryLocator _groupContextFactoryLocator;
    private IRemoteNntpClient _client;
    private ConcurrentDictionary<string, ArticleStreamWriter> _writers = new();

    public ArticleDownloader(GroupContextFactoryLocator groupContextFactoryLocator, IRemoteNntpClient client)
    {
        _groupContextFactoryLocator = groupContextFactoryLocator;
        _client = client;
    }

    private ArticleStreamWriter GetWriter(ArticleBlobFile blobFile)
        => _writers.GetOrAdd(blobFile.FilePath, x => new ArticleStreamWriter(blobFile));

    private ArticleBlobFile GetBlobFile(Group group, ArticleHeader articleHeader)
    {
        var groupDir = DirHelper.GetGroupDataDir(group);
        return ArticleBlobFile.Create(group, 1);
    }

    public async Task<int> DownloadArticles(Group group, int limit = 1000)
    {
        var ctx = _groupContextFactoryLocator.GetGroupDbContextFactory(group).CreateDbContext();
        var headers = await ctx.ArticleHeader
            .Where(x => x.BlobId == null)
            .Take(limit)
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        if (headers.Count == 0)
            return 0;

        var n = 0;
        foreach (var header in headers)
        {
            var blobFile = GetBlobFile(group, header);
            var writer = GetWriter(blobFile);
            var article = _client.GetArticle(header.MessageId);
            var appendResult = await writer.AppendArticle(header.Id, article);
            header.BlobId = blobFile.BlobId;
            header.BlogOffset = appendResult.ArticleOffset;
            await ctx.SaveChangesAsync();
            n++;
        }

        return n;
    }
}