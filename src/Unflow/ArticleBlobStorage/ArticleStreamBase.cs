namespace Unflow.ArticleBlobStorage;

public class ArticleStreamBase : IDisposable, IAsyncDisposable
{
    public ArticleBlobFile ArticleBlobFile { get; }
    public Guid FileId => _articleBlobFileHeader.FileId;
    private protected ICompressor _compressor;
    private protected ArticleBlobFileHeader _articleBlobFileHeader;
    private protected AsyncLock _asyncLock = new AsyncLock();
    private protected Stream _stream;

    public ArticleStreamBase(ArticleBlobFile articleBlobFile)
    {
        ArticleBlobFile = articleBlobFile;
    }

    private protected ArticleBlobFileHeader ReadHeader(Stream readerStream)
    {
        var _reader = new BinaryReader(readerStream);
        var header = new ArticleBlobFileHeader
        {
            Flags = (ArticleBlobFileFlags)_reader.ReadInt32(),
            FileId = new Guid(_reader.ReadBytes(16)),
            Reserved = _reader.ReadBytes(16),
        };

        return header;
    }

    public virtual void Dispose()
    {
        _stream?.Dispose();
        _stream = null!;
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (_stream == null)
            return;

        await _stream.DisposeAsync();
        _stream = null!;
    }
}