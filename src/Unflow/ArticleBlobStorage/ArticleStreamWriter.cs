using Joveler.Compression.XZ.Checksum;

namespace Unflow.ArticleBlobStorage;

public class ArticleStreamWriter
{
    public void AppendArticle(Article articleHeader)
    {
        var article = articleHeader.ToString(new HeaderEncoding());
        //var data = 
    }
}