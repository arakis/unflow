namespace Unflow.ArticleBlobStorage;

public class ArticleBlobFileHeader
{
    public ArticleBlobFileFlags Flags { get; set; }
    public Guid FileId { get; set; }
    public byte[] Reserved { get; set; } = new byte [16];
}