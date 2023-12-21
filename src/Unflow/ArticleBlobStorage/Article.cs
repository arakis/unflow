namespace Unflow.ArticleBlobStorage;

public class Article
{
    public string[] Headers { get; set; } = null!;
    public string[] Body { get; set; } = null!;

    public string ToString(HeaderEncoding headerEncoding)
        => string.Join("\n", Headers.Select(x => headerEncoding.Encode(x))) + "\n" + string.Join("\n", Body);

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