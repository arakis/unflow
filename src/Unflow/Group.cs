using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Group.Id), IsUnique = true)]
[Index(nameof(Group.Name))]
public class Group
{
    public Guid Id { get; set; }

    [MaxLength(250)] // TODO: Max Length
    public string Name { get; set; }

    public long LowestDownloadedArticleNumber { get; set; }
    public long HighestDownloadedArticleNumber { get; set; }

    public ArticleRange GetArticleRange()
        => ArticleRange.FromStartEnd(LowestDownloadedArticleNumber, HighestDownloadedArticleNumber);
}