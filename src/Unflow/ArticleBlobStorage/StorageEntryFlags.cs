namespace Unflow.ArticleBlobStorage;

[Flags]
public enum StorageEntryFlags : int
{
    None = 0,
    Compressed = 16,
}