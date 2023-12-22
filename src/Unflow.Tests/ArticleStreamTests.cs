using JetBrains.Annotations;
using Joveler.Compression.XZ;
using Unflow.ArticleBlobStorage;

namespace Unflow.Tests.ArticleBlobStorage;

[TestSubject(typeof(ArticleStreamReader))]
[TestSubject(typeof(ArticleStreamWriter))]
public class ArticleStreamTests
{
    private static readonly string _testDir;

    static ArticleStreamTests()
    {
        _testDir = Path.Combine("/tmp/unflow-tests");
        Directory.CreateDirectory(_testDir);
    }

    [Fact]
    public void BlobFileHeaderTest()
    {
        var fileName = $"{_testDir}/{nameof(ArticleStreamTests)}-{nameof(BlobFileHeaderTest)}.bin";
        var blobFile = new ArticleBlobFile()
        {
            FilePath = fileName
        };

        Guid blobId;
        using (var articleStreamWriter = new ArticleStreamWriter(blobFile))
        {
            blobId = articleStreamWriter.FileId;
        }

        Guid blobId2;
        using (var articleStreamWriter = new ArticleStreamWriter(blobFile))
        {
            blobId2 = articleStreamWriter.FileId;
        }

        var fileInfo = new FileInfo(fileName);
        Assert.Equal(36, fileInfo.Length);
        Assert.Equal(blobId, blobId2);
    }

    [Fact]
    public async Task AddTest()
    {
        var fileName = $"{_testDir}/{nameof(ArticleStreamTests)}-{nameof(AddTest)}.bin";
        if (File.Exists(fileName))
            File.Delete(fileName);

        var blobFile = new ArticleBlobFile()
        {
            FilePath = fileName
        };

        AppendArticleResult appendResulst1;
        using (var articleStreamWriter = new ArticleStreamWriter(blobFile))
        {
            var id = Guid.NewGuid();
            var article = new Article()
            {
                Headers = new[] { "From: plain@text.local", "Subject: no compression" },
                Body = new[]
                {
                    "do not compress me",
                    "qwertzuiopasdfghjklyxcvbnm"
                }
            };
            appendResulst1 = await articleStreamWriter.AppendArticle(id, article);
        }

        Assert.Equal(36, appendResulst1.ArticleOffset);

        AppendArticleResult appendResulst2;
        using (var articleStreamWriter = new ArticleStreamWriter(blobFile))
        {
            var id = Guid.NewGuid();
            var article = new Article()
            {
                Headers = new[] { "From: compress@me.local", "Subject: compress me" },
                Body = new[]
                {
                    "compress me",
                    "compress me",
                    "compress me",
                    "compress me",
                    "compress me",
                    "compress me",
                    "compress me",
                    "compress me",
                }
            };
            appendResulst2 = await articleStreamWriter.AppendArticle(id, article);
        }

        Assert.True(appendResulst2.ArticleOffset > appendResulst1.ArticleOffset);
        
        var reader= new ArticleStreamReader(blobFile);
        var article1 = await reader.ReadArticle(appendResulst1.ArticleOffset);
        var article2 = await reader.ReadArticle(appendResulst2.ArticleOffset);
    }
}