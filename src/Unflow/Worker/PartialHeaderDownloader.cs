using Org.BouncyCastle.Crypto.Digests;
using Unflow.Cli;

public class PartialHeaderDownloader
{
    private IRemoteNntpClient _client;
    private int _chunkSize;

    public PartialHeaderDownloader(IRemoteNntpClient client, int chunkSize = 1000)
    {
        _client = client;
        _chunkSize = chunkSize;
    }

    public void DownloadPartialHeaders(RemoteGroupInfo remoteGroupInfo, Group group)
    {
        var localIsEmpty = group.GetArticleRange().IsDefault;
        if (!localIsEmpty)
        {
            // Download newest headers
            var highRange = ArticleRange.FromStartEnd(group.HighestDownloadedArticleNumber + 1, remoteGroupInfo.HighestAvailableArticleNumber);
            DownloadNewestPartialHeaders(group, remoteGroupInfo, highRange);
        }

        // Download oldest headers
        ArticleRange lowRange;
        if (localIsEmpty)
            lowRange = remoteGroupInfo.GetArticleRange();
        else
            lowRange = ArticleRange.FromStartEnd(remoteGroupInfo.LowestAvailableArticleNumber, group.LowestDownloadedArticleNumber - 1);

        if (lowRange.Length > 0)
            DownloadOldestPartialHeaders(group, remoteGroupInfo, lowRange);
    }

    private void DownloadNewestPartialHeaders(Group group, RemoteGroupInfo remoteGroupInfo, ArticleRange range)
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
                ProcessPartialHeader(partialHeader);

                // Update localGroupInfo
                if (group.HighestDownloadedArticleNumber == 0 || partialHeader.ArticleNumber > group.HighestDownloadedArticleNumber)
                    group.HighestDownloadedArticleNumber = partialHeader.ArticleNumber;
                
                if(group.LowestDownloadedArticleNumber == 0)
                    group.LowestDownloadedArticleNumber = group.HighestDownloadedArticleNumber;
            }

            currentRange = currentRange.Move(_chunkSize).Intersection(range);
        }
    }

    private void DownloadOldestPartialHeaders(Group group, RemoteGroupInfo remoteGroupInfo, ArticleRange range)
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
                ProcessPartialHeader(partialHeader);

                // Update localGroupInfo
                if (group.LowestDownloadedArticleNumber == 0 || partialHeader.ArticleNumber < group.LowestDownloadedArticleNumber)
                    group.LowestDownloadedArticleNumber = partialHeader.ArticleNumber;

                if (group.HighestDownloadedArticleNumber == 0)
                    group.HighestDownloadedArticleNumber = group.LowestDownloadedArticleNumber;
            }

            currentRange = currentRange.Move(-_chunkSize).Intersection(range);
        }
    }

    private void ProcessPartialHeader(RemotePartialHeader remotePartialHeader)
    {
    }
}