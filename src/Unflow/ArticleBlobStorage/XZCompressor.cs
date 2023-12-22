using Joveler.Compression.XZ;

namespace Unflow.ArticleBlobStorage;

public class XZCompressor : ICompressor
{
    static XZCompressor()
    {
        XZInitSingleton.Init();
    }

    public async Task<byte[]> Compress(byte[] inputData)
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
                    await fsOrigin.CopyToAsync(zs);
                    await zs.FlushAsync();
                }

                return fsComp.ToArray();
            }
        }
    }

    public async Task<byte[]> Decompress(byte[] inputData)
    {
        XZDecompressOptions decompOpts = new XZDecompressOptions();

        using (var fsComp = new MemoryStream(inputData))
        {
            using (var fsDecomp = new MemoryStream())
            {
                using (var zs = new XZStream(fsComp, decompOpts))
                {
                    await zs.CopyToAsync(fsDecomp);
                    await zs.FlushAsync();
                }

                return fsDecomp.ToArray();
            }
        }
    }
}