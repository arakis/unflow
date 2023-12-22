using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Digests;
using Unflow;
using Unflow.Cli;

public class PartialHeaderDownloader
{
    private readonly GroupContextFactoryLocator _groupContextFactoryLocator;
    private IRemoteNntpClient _client;
    private int _chunkSize;

    public PartialHeaderDownloader(GroupContextFactoryLocator groupContextFactoryLocator, IRemoteNntpClient client, int chunkSize = 1000)
    {
        _groupContextFactoryLocator = groupContextFactoryLocator;
        _client = client;
        _chunkSize = chunkSize;
    }

    public async Task DownloadPartialHeaders(RemoteGroupInfo remoteGroupInfo, Group group)
    {
        using var groupDbContext = _groupContextFactoryLocator.GetGroupDbContextFactory(group).CreateDbContext();
        var localIsEmpty = group.GetArticleRange().IsDefault;
        if (!localIsEmpty)
        {
            // Download newest headers
            var highRange = ArticleRange.FromStartEnd(group.HighestDownloadedArticleNumber + 1, remoteGroupInfo.HighestAvailableArticleNumber);
            await DownloadNewestPartialHeaders(group, remoteGroupInfo, highRange, groupDbContext);
        }

        // Download oldest headers
        ArticleRange lowRange;
        if (localIsEmpty)
            lowRange = remoteGroupInfo.GetArticleRange();
        else
            lowRange = ArticleRange.FromStartEnd(remoteGroupInfo.LowestAvailableArticleNumber, group.LowestDownloadedArticleNumber - 1);

        if (lowRange.Length > 0)
            await DownloadOldestPartialHeaders(group, remoteGroupInfo, lowRange, groupDbContext);
    }

    private async Task DownloadNewestPartialHeaders(Group group, RemoteGroupInfo remoteGroupInfo, ArticleRange range, GroupDbContext groupDbContext)
    {
        var remoteRange = remoteGroupInfo.GetArticleRange();
        range = range.Intersection(remoteRange);

        var currentRange = range;
        currentRange = currentRange.TakeFirst(_chunkSize);

        while (currentRange.Length > 0)
        {
            var partialHeaders = _client.GetPartialHeaderForGroup(group.Name, currentRange).OrderBy(a => a.ArticleNumber).ToList();

            foreach (var partialHeader in partialHeaders)
            {
                ProcessPartialHeader(partialHeader, groupDbContext);

                // Update localGroupInfo
                if (group.HighestDownloadedArticleNumber == 0 || partialHeader.ArticleNumber > group.HighestDownloadedArticleNumber)
                    group.HighestDownloadedArticleNumber = partialHeader.ArticleNumber;

                if (group.LowestDownloadedArticleNumber == 0)
                    group.LowestDownloadedArticleNumber = group.HighestDownloadedArticleNumber;
            }

            currentRange = currentRange.Move(_chunkSize).Intersection(range);
        }

        await groupDbContext.SaveChangesAsync();
    }

    private async Task DownloadOldestPartialHeaders(Group group, RemoteGroupInfo remoteGroupInfo, ArticleRange range, GroupDbContext groupDbContext)
    {
        var remoteRange = remoteGroupInfo.GetArticleRange();
        range = range.Intersection(remoteRange);

        var currentRange = range;
        currentRange = currentRange.TakeLast(_chunkSize);

        while (currentRange.Length > 0)
        {
            var partialHeaders = _client.GetPartialHeaderForGroup(group.Name, currentRange).OrderByDescending(a => a.ArticleNumber).ToList();

            foreach (var partialHeader in partialHeaders)
            {
                ProcessPartialHeader(partialHeader, groupDbContext);

                // Update localGroupInfo
                if (group.LowestDownloadedArticleNumber == 0 || partialHeader.ArticleNumber < group.LowestDownloadedArticleNumber)
                    group.LowestDownloadedArticleNumber = partialHeader.ArticleNumber;

                if (group.HighestDownloadedArticleNumber == 0)
                    group.HighestDownloadedArticleNumber = group.LowestDownloadedArticleNumber;
            }

            currentRange = currentRange.Move(-_chunkSize).Intersection(range);
        }

        await groupDbContext.SaveChangesAsync();
    }

    private async Task ProcessPartialHeader(RemotePartialHeader remotePartialHeader, GroupDbContext groupDbContext)
    {
        var article = await groupDbContext.ArticleHeader.FirstOrDefaultAsync(x => x.ArticleNumber == remotePartialHeader.ArticleNumber);
        if (article == null)
        {
            article = new ArticleHeader()
            {
                Id = SecureGuidGenerator.Default.NewGuid(),
                ArticleNumber = remotePartialHeader.ArticleNumber,
                Author = remotePartialHeader.Author,
                Date = remotePartialHeader.Date,
                MessageId = remotePartialHeader.MessageId,
                References = remotePartialHeader.References,
                Subject = remotePartialHeader.Subject,
                HeaderDownloadedAt = DateTimeOffset.UtcNow
            };
            groupDbContext.ArticleHeader.Add(article);
        }
    }
}