using Microsoft.Extensions.Logging;
using Unflow.ArticleBlobStorage;
using Usenet.Nntp.Parsers;

internal class CustomArticleResponseParser : IMultiLineResponseParser<CustomNntpArticleResponse>
{
    private readonly ILogger? _logger;
    private readonly CustomArticleRequestType requestType;
    private readonly int successCode;

    public CustomArticleResponseParser(ILogger? logger, CustomArticleRequestType requestType)
    {
        _logger = logger;
        switch (this.requestType = requestType)
        {
            case CustomArticleRequestType.Head:
                this.successCode = 221;
                break;
            case CustomArticleRequestType.Body:
                this.successCode = 222;
                break;
            case CustomArticleRequestType.Article:
                this.successCode = 220;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(requestType), (object)requestType, (string)null);
        }
    }

    public bool IsSuccessResponse(int code) => code == this.successCode;

    public CustomNntpArticleResponse Parse(int code, string message, IEnumerable<string> dataBlock)
    {
        if (!this.IsSuccessResponse(code))
            return new CustomNntpArticleResponse(code, message, false, null, null);
        string[] strArray = message.Split(new char[1] { ' ' });
        if (strArray.Length < 2)
            this._logger?.LogError("Invalid response message: {Message} Expected: {{number}} {{messageid}}", (object)message);
        long result;
        long.TryParse(strArray.Length != 0 ? strArray[0] : (string)null, out result);
        string messageId = strArray.Length > 1 ? strArray[1] : string.Empty;
        if (dataBlock == null)
            return new CustomNntpArticleResponse(code, message, true, new Article() { Body = Array.Empty<string>(), Headers = Array.Empty<string>() }, messageId);

        var article = Article.ParseNative(dataBlock);
        return new CustomNntpArticleResponse(code, message, true, article, messageId);
    }
}