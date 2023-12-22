namespace Unflow.ArticleBlobStorage;

public class HeaderEncoding
{
    static HeaderEncoding()
    {
        AddKnownHeader("p", "Path");
        AddKnownHeader("f", "From");
        AddKnownHeader("n", "Newsgroups");
        AddKnownHeader("s", "Subject");
        AddKnownHeader("d", "Date");
        AddKnownHeader("L", "Lines");
        AddKnownHeader("i", "Message-ID");
        AddKnownHeader("r", "References");
        AddKnownHeader("v", "Mime-Version");
        AddKnownHeader("t", "Content-Type");
        AddKnownHeader("e", "Content-Transfer-Encoding");
        AddKnownHeader("z", "X-Trace");
        AddKnownHeader("c", "Cancel-Lock");
        AddKnownHeader("u", "User-Agent");
        AddKnownHeader("l", "Content-Language");
        AddKnownHeader("y", "In-Reply-To");
        AddKnownHeader("F", "Xref");
        AddKnownHeader("o", "Organization");
        AddKnownHeader("w", "Followup-To");
        AddKnownHeader("P", "X-Orig-Path");
    }

    private static void AddKnownHeader(string encodedName, string decodedName)
    {
        _DecodedToEncoded.Add(decodedName, encodedName);
        _EncodedToDecoded.Add(encodedName, decodedName);
    }

    private static Dictionary<string, string> _DecodedToEncoded = new();
    private static Dictionary<string, string> _EncodedToDecoded = new();

    public string Encode(string originalHeader)
    {
        var idx = originalHeader.IndexOf(':');
        if (idx == -1)
            return originalHeader.TrimEnd();

        var key = originalHeader.Substring(0, idx);

        if (_DecodedToEncoded.TryGetValue(key, out var encodedKey))
            return encodedKey + ";" + (originalHeader.Substring(idx + 1).Trim());

        return originalHeader.Trim(); // unknown header
    }

    public string Decode(string encodedHeader)
    {
        encodedHeader = encodedHeader.TrimEnd();
        var idx = encodedHeader.IndexOf(';');
        if (idx == -1)
            return encodedHeader;

        var key = encodedHeader.Substring(0, idx);

        if (_EncodedToDecoded.TryGetValue(key, out var decodedKey))
            return decodedKey + ": " + encodedHeader.Substring(idx + 1);

        return encodedHeader;
    }
}