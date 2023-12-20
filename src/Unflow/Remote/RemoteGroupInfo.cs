public class RemoteGroupInfo
{
    public string Name { get; set; }
    public long LowestAvailableArticleNumber { get; set; }
    public long HighestAvailableArticleNumber { get; set; }

    public ArticleRange GetArticleRange()
        => ArticleRange.FromStartEnd(LowestAvailableArticleNumber, HighestAvailableArticleNumber);
}