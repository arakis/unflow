using System.Text;
using Joveler.Compression.XZ.Checksum;

namespace Unflow.ArticleBlobStorage;

public class ArticleStreamWriter : ArticleStreamBase
{
    public ArticleStreamWriter(ArticleBlobFile articleBlobFile)
        : base(articleBlobFile)
    {
        _compressor = new XZCompressor();
        bool created;
        if (File.Exists(articleBlobFile.FilePath))
        {
            created = false;
            // open single writer (append), multi reader
            _stream = File.Open(articleBlobFile.FilePath, FileMode.Open, FileAccess.Write, FileShare.Read);
            _stream.Seek(0, SeekOrigin.End);
        }
        else
        {
            created = true;
            // create new file
            _stream = File.Open(articleBlobFile.FilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        }

        _writer = new BinaryWriter(_stream);

        ArticleBlobFileHeader header;
        if (created)
        {
            header = new ArticleBlobFileHeader
            {
                Flags = ArticleBlobFileFlags.None,
                FileId = SecureGuidGenerator.Default.NewGuid(),
            };

            WriteHeader(header);
        }
        else
        {
            using var readerStream = File.Open(ArticleBlobFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            header = ReadHeader(readerStream);
        }

        _articleBlobFileHeader = header;
    }

    private BinaryWriter _writer;

    private void WriteHeader(ArticleBlobFileHeader header)
    {
        _writer.Write((int)header.Flags);
        _writer.Write(header.FileId.ToByteArray());
        _writer.Write(header.Reserved);
    }

    public async Task<AppendArticleResult> AppendArticle(Guid articleId, Article articleHeader)
    {
        using (await _asyncLock.LockAsync())
            return await AppendArticleInternal(articleId, articleHeader);
    }

    private async Task<AppendArticleResult> AppendArticleInternal(Guid articleId, Article articleHeader)
    {
        var offset = _stream.Position;
        var article = articleHeader.ToString(new HeaderEncoding());

        var originalData = Encoding.UTF8.GetBytes(article);
        var compressedData = await _compressor.Compress(originalData);

        byte[] data;

        StorageEntryFlags flags = StorageEntryFlags.None;

        if (compressedData.Length > originalData.Length)
        {
            data = originalData;
        }
        else
        {
            flags |= StorageEntryFlags.Compressed;
            data = compressedData;
        }

        var hashBytes = new Crc32Algorithm().ComputeHash(data);
        var checksum = BitConverter.ToInt32(hashBytes);

        var entry = new StorageEntry
        {
            Flags = flags,
            Id = articleId,
            Data = data,
            DataLength = data.Length,
            DataChecksum = checksum,
        };

        await WriteEntry(entry);

        return new AppendArticleResult { ArticleOffset = offset };
    }

    private async Task WriteEntry(StorageEntry entry)
    {
        _writer.Write((int)entry.Flags);
        _writer.Write(entry.Id.ToByteArray());
        _writer.Write(entry.DataLength);
        _writer.Write(entry.DataChecksum);
        _writer.Write(entry.Data);

        await _stream.FlushAsync();
    }

    public void Dispose()
    {
        _writer = null!;
        base.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        _writer = null!;
        return base.DisposeAsync();
    }
}