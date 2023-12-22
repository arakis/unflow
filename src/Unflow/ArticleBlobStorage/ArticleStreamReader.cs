using System.Text;
using Joveler.Compression.XZ.Checksum;

namespace Unflow.ArticleBlobStorage;

public class ArticleStreamReader : ArticleStreamBase
{
    public ArticleStreamReader(ArticleBlobFile articleBlobFile)
        : base(articleBlobFile)
    {
        _compressor = new XZCompressor();
        // open single reader, regardles of other readers/writers
        _stream = File.Open(articleBlobFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        _reader = new BinaryReader(_stream);

        ArticleBlobFileHeader header;
        header = ReadHeader(_stream);

        _articleBlobFileHeader = header;
    }

    private BinaryReader _reader;

    public async Task<Article> ReadArticle(long offset, bool validateChecksum = true)
    {
        using (await _asyncLock.LockAsync())
            return await ReadArticleInternal(offset, validateChecksum);
    }

    private async Task<Article> ReadArticleInternal(long offset, bool validateChecksum = true)
    {
        _stream.Seek(offset, SeekOrigin.Begin);
        var entry = await ReadEntry(offset);

        if (validateChecksum)
        {
            var hashBytes = new Crc32Algorithm().ComputeHash(entry.Data);
            var checksum = BitConverter.ToInt32(hashBytes);

            if (checksum != entry.DataChecksum)
                throw new Exception($"Checksum mismatch. Offset: {offset}, checksum: {checksum}, expected: {entry.DataChecksum}, File path: {ArticleBlobFile.FilePath}");
        }

        byte[] data;
        if (entry.Flags.HasFlag(StorageEntryFlags.Compressed))
            data = await _compressor.Decompress(entry.Data);
        else
            data = entry.Data;

        var articleString = Encoding.UTF8.GetString(data);
        var article = Article.Parse(articleString, new HeaderEncoding());

        return article;
    }

    private Task<StorageEntry> ReadEntry(long offset)
    {
        return Task.Run(() =>
        {
            var entry = new StorageEntry();
            _stream.Seek(offset, SeekOrigin.Begin);

            entry.Flags = (StorageEntryFlags)_reader.ReadInt32();
            entry.Id = new Guid(_reader.ReadBytes(16));
            entry.DataLength = _reader.ReadInt32();
            entry.DataChecksum = _reader.ReadInt32();
            entry.Data = _reader.ReadBytes(entry.DataLength);

            return entry;
        });
    }

    public void Dispose()
    {
        _reader = null!;
        base.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        _reader = null!;
        return base.DisposeAsync();
    }
}