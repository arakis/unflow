using Microsoft.EntityFrameworkCore;

[Index(nameof(ArticleHeader.Id), IsUnique = true)]
[Index(nameof(ArticleHeader.MessageId), IsUnique = true)]
public class ArticleHeader
{
    public Guid Id { get; set; }

    public string References { get; set; }

    public string MessageId { get; set; }

    public DateTimeOffset Date { get; set; }

    public string Author { get; set; }

    public string Subject { get; set; }

    public int ArticleNumber { get; set; }

    public DateTimeOffset HeaderDownloadedAt { get; set; }
    public DateTimeOffset? BodyDownloadedAt { get; set; }

    public int? BlobId { get; set; }
    public int? BlogOffset { get; set; }
    public Guid? ParentId { get; set; }
}