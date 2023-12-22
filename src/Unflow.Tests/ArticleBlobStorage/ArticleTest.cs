using Xunit;
using Unflow.ArticleBlobStorage;

namespace Unflow.Tests.ArticleBlobStorage
{
    public class ArticleTest
    {
        [Fact]
        public void TestToString()
        {
            var headerEncoding = new HeaderEncoding();
            var article = new Article
            {
                Headers = new[] { "header1: value1", "header2: value2" },
                Body = new[] { "line1", "line2" }
            };
            var expected = "header1: value1\nheader2: value2\n\nline1\nline2";
            Assert.Equal(expected, article.ToString(headerEncoding));
        }

        [Fact]
        public void TestToStringWithEmptyBody()
        {
            var headerEncoding = new HeaderEncoding();
            var article = new Article
            {
                Headers = new[] { "header1: value1", "header2: value2" },
                Body = new string[0]
            };
            var expected = "header1: value1\nheader2: value2";
            Assert.Equal(expected, article.ToString(headerEncoding));
        }

        [Fact]
        public void TestParse()
        {
            var headerEncoding = new HeaderEncoding();
            var text = "header1: value1\nheader2: value2\n\nline1\nline2";
            var article = Article.Parse(text, headerEncoding);
            Assert.Equal(new[] { "header1: value1", "header2: value2" }, article.Headers);
            Assert.Equal(new[] { "line1", "line2" }, article.Body);
        }

        [Fact]
        public void TestParseWithEmptyBody()
        {
            var headerEncoding = new HeaderEncoding();
            var text = "header1: value1\nheader2: value2";
            var article = Article.Parse(text, headerEncoding);
            Assert.Equal(new[] { "header1: value1", "header2: value2" }, article.Headers);
            Assert.Empty(article.Body);
        }

        [Fact]
        public void TestParseWithOnlyHeaders()
        {
            var headerEncoding = new HeaderEncoding();
            var text = "header1: value1\nheader2: value2";
            var article = Article.Parse(text, headerEncoding);
            Assert.Equal(new[] { "header1: value1", "header2: value2" }, article.Headers);
            Assert.Empty(article.Body);
        }

        [Fact]
        public void TestParseWithOnlyBody()
        {
            var headerEncoding = new HeaderEncoding();
            var text = "\n\nline1\nline2";
            var article = Article.Parse(text, headerEncoding);
            Assert.Empty(article.Headers);
            Assert.Equal(new[] { "line1", "line2" }, article.Body);
        }
    }
}