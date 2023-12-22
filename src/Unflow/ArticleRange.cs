using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualBasic;

public readonly struct ArticleRange
{
    public readonly long Start;
    public readonly long End;
    public long Length => End - Start + 1;

    [SetsRequiredMembers]
    private ArticleRange(long start, long end)
    {
        Start = start;
        End = end;
    }

    private ArticleRange FromStartLength(long start, long length)
        => new(start, start + length - 1);

    public static ArticleRange FromStartEnd(long start, long end)
        => new(start, end);

    public ArticleRange Intersection(ArticleRange other)
    {
        var start = Math.Max(Start, other.Start);
        var end = Math.Min(End, other.End);
        return ArticleRange.FromStartEnd(start, end);
    }

    public ArticleRange Intersection(long low, long high)
    {
        var start = Math.Max(Start, low);
        var end = Math.Min(End, high);
        return ArticleRange.FromStartEnd(start, end);
    }

    public ArticleRange Move(long steps)
        => FromStartLength(Start + steps, Length);

    public ArticleRange TakeLast(long steps)
    {
        var start = Math.Max(Start, End - steps + 1);
        return ArticleRange.FromStartEnd(start, End);
    }

    public ArticleRange TakeFirst(long steps)
    {
        var end = Math.Min(End, Start + steps - 1);
        return ArticleRange.FromStartEnd(Start, end);
    }

    public ArticleRange RemoveLast(long steps)
    {
        var start = Start;
        var end = Math.Max(Start, End - steps);
        return ArticleRange.FromStartEnd(start, end);
    }

    public bool IsDefault
        => Start == 0 && End == 0;

    public bool IsEmpty
        => Length == 0;

    public override string ToString()
        => $"[{Start}..{End}, Length={Length}]";
}