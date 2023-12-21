using Joveler.Compression.XZ;

namespace Unflow.ArticleBlobStorage;

public class XZCompressor : ICompressor
{
    static XZCompressor()
    {
        XZInit.GlobalInit();
    }

    public byte[] Compress(byte[] inputData)
    {
        // Compress in single-threaded mode
        XZCompressOptions compOpts = new XZCompressOptions
        {
            Level = LzmaCompLevel.Default,
        };

        using (var fsOrigin = new MemoryStream(inputData))
        using (var fsComp = new MemoryStream())
        using (var zs = new XZStream(fsComp, compOpts))
        {
            fsOrigin.CopyTo(zs);
            zs.Flush();
            return fsComp.ToArray();
        }
    }

    public byte[] Decompress(byte[] inputData)
    {
        XZDecompressOptions decompOpts = new XZDecompressOptions();

        using (var fsDecomp = new MemoryStream())
        using (var fsComp = new MemoryStream(inputData))
        using (var zs = new XZStream(fsComp, decompOpts))
        {
            zs.CopyTo(fsDecomp);
            fsDecomp.Flush();
            return fsDecomp.ToArray();
        }
    }
}