namespace Unflow.ArticleBlobStorage;

public class StorageEntry
{
    public StorageEntryFlags Flags { get; set; }
    public Guid Id { get; set; }
    public int DataLength { get; set; }
    public int DataChecksum { get; set; }
    public byte[] Data { get; set; }
}

