using JetBrains.Annotations;
using Unflow.ArticleBlobStorage;

namespace Unflow.Tests;

[TestSubject(typeof(ArticleRange))]
public class ArticleRangeTests
{
    [Fact]
    public void SameStartEndShouldHaveLengthOfOne()
    {
        var range = ArticleRange.FromStartEnd(5, 5);
        Assert.Equal(1, range.Length);
    }
    
    [Fact]
    public void IntersectionInside()
    {
        var range1 = ArticleRange.FromStartEnd(5, 10);
        var range2 = ArticleRange.FromStartEnd(8, 15);
        var intersection = range1.Intersection(range2);
        Assert.Equal(8, intersection.Start);
        Assert.Equal(10, intersection.End);
    }

    [Fact]
    public void IntersectionExact()
    {
        var range1 = ArticleRange.FromStartEnd(5, 10);
        var range2 = ArticleRange.FromStartEnd(5, 10);
        var intersection = range1.Intersection(range2);
        Assert.Equal(5, intersection.Start);
        Assert.Equal(10, intersection.End);
    }

    [Fact]
    public void NoIntersection()
    {
        var range1 = ArticleRange.FromStartEnd(5, 10);
        var range2 = ArticleRange.FromStartEnd(11, 15);
        var intersection = range1.Intersection(range2);
        Assert.Equal(0, intersection.Length);
    }

    [Fact]
    public void Move()
    {
        var range = ArticleRange.FromStartEnd(5, 10);
        var moved = range.Move(2);
        Assert.Equal(7, moved.Start);
        Assert.Equal(12, moved.End);
    }
    
    [Fact]
    public void TakeLastSmall()
    {
        var range = ArticleRange.FromStartEnd(5, 10);
        var taken = range.TakeLast(2);
        Assert.Equal(9, taken.Start);
        Assert.Equal(10, taken.End);
    }

    [Fact]
    public void TakeLastLarge()
    {
        var range = ArticleRange.FromStartEnd(5, 10);
        var taken = range.TakeLast(20);
        Assert.Equal(5, taken.Start);
        Assert.Equal(10, taken.End);
    }

    [Fact]
    public void TakeFirstSmall()
    {
        var range = ArticleRange.FromStartEnd(5, 10);
        var taken = range.TakeFirst(2);
        Assert.Equal(5, taken.Start);
        Assert.Equal(6, taken.End);
    }

    [Fact]
    public void TakeFirstLarge()
    {
        var range = ArticleRange.FromStartEnd(5, 10);
        var taken = range.TakeFirst(20);
        Assert.Equal(5, taken.Start);
        Assert.Equal(10, taken.End);
    }
}