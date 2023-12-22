using Unflow.ArticleBlobStorage;
using Usenet.Nntp.Responses;

public class CustomNntpArticleResponse : NntpResponse
{
    public Article? Article { get; }
    public string? MessageId { get; }

    public CustomNntpArticleResponse(int code, string message, bool success, Article? article, string? messageId)
        : base(code, message, success)
    {
        this.Article = article;
        this.MessageId = messageId;
    }
}