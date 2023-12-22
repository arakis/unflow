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
            Level = LzmaCompLevel.Level9,
            ExtremeFlag = true,
        };

        using (var fsOrigin = new MemoryStream(inputData))
        {
            using (var fsComp = new MemoryStream())
            {
                using (XZStream zs = new XZStream(fsComp, compOpts))
                {
                    fsOrigin.CopyTo(zs);
                    zs.Flush();
                }

                return fsComp.ToArray();
            }
        }
    }

    public byte[] Decompress(byte[] inputData)
    {
        XZDecompressOptions decompOpts = new XZDecompressOptions();

        using (var fsComp = new MemoryStream(inputData))
        {
            using (var fsDecomp = new MemoryStream())
            {
                using (var zs = new XZStream(fsComp, decompOpts))
                {
                    zs.CopyTo(fsDecomp);
                    zs.Flush();
                }

                return fsDecomp.ToArray();
            }
        }
    }
}