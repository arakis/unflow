namespace Unflow.Cli;

public class RemotePartialHeader
{
    public bool Valid { get; private set; }

    public RemotePartialHeader(string line)
    {
        // Article number: 12345
        // Subject: Re: Example Subject
        // Author: John Doe <johndoe@example.com>
        // Date: 12 Oct 2020 12:34:56 GMT
        // Message-ID: <1234abcd@example.com>
        // References: <2345bcde@example.com>
        // Byte count: 4567
        // Line count: 89

        Valid = true;
        var parts = line.Split('\t');

        ArticleNumber = int.Parse(parts[0]);
        Subject = parts[1];
        Author = parts[2];

        if (MimeKit.Utils.DateUtils.TryParse(parts[3], out var date))
            Date = date;
        else
            Valid = false;

        MessageId = parts[4];
        References = parts[5];
        ByteCount = int.Parse(parts[6]);
        LineCount = int.Parse(parts[7]);
    }

    public int LineCount { get; set; }

    public int ByteCount { get; set; }

    public string References { get; set; }

    public string MessageId { get; set; }

    public DateTimeOffset Date { get; set; }

    public string Author { get; set; }

    public string Subject { get; set; }

    public int ArticleNumber { get; set; }
}