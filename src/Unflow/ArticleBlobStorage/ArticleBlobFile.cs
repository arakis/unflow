namespace Unflow.ArticleBlobStorage;

public class ArticleBlobFile
{
    public string FilePath { get; set; }
    public int BlobId { get; set; }

    public static ArticleBlobFile Create(Group group, int blobId)
    {
        var groupDir = DirHelper.GetGroupDataDir(group);
        var blobFile = new ArticleBlobFile
        {
            FilePath = Path.Combine(groupDir, $"articles-{blobId}.bin"),
            BlobId = blobId,
        };
        return blobFile;
    }
}