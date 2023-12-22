using System.Text;

namespace Unflow.ArticleBlobStorage;

public class Article
{
    public string[] Headers { get; set; } = null!;
    public string[] Body { get; set; } = null!;

    public string ToString(HeaderEncoding headerEncoding)
    {
        var sb = new StringBuilder();
        sb.Append(string.Join("\n", Headers.Select(x => headerEncoding.Encode(x))));
        if (Body.Length > 0)
            sb.Append("\n\n");
        sb.Append(string.Join("\n", Body));
        return sb.ToString();
    }

    public static Article ParseNative(IEnumerable<string> lines)
    {
        var headers = new List<string>();
        var body = new List<string>();

        var addToBody = false;
        foreach (var line in lines)
        {
            if (line == string.Empty)
                addToBody = true;
            else if (addToBody)
                body.Add(line);
            else
                headers.Add(line);
        }

        return new Article
        {
            Headers = headers.ToArray(),
            Body = body.ToArray()
        };
    }

    public static Article Parse(string text, HeaderEncoding headerEncoding)
    {
        var lines = text.Split('\n');
        var headers = new List<string>();
        var body = new List<string>();

        var addToBody = false;
        foreach (var line in lines)
        {
            if (line == string.Empty)
                addToBody = true;
            else if (addToBody)
                body.Add(line);
            else
                headers.Add(headerEncoding.Decode(line));
        }

        return new Article
        {
            Headers = headers.ToArray(),
            Body = body.ToArray()
        };
    }
}