using JetBrains.Annotations;
using Joveler.Compression.XZ;
using Unflow.ArticleBlobStorage;

namespace Unflow.Tests.ArticleBlobStorage;

[TestSubject(typeof(XZStream))]
public class XZLibTests
{
    static XZLibTests()
    {
        XZInitSingleton.Init();
    }

    [Fact]
    public void Test()
    {
        var testDir = "/tmp/unflow-tests";
        Directory.CreateDirectory(testDir);
        var fileOriginal = $"{testDir}/file_origin.bin";
        var fileCompressed = $"{testDir}/test.xz";
        var fileDecompressed = $"{testDir}/file_decomp.bin";

        File.WriteAllText(fileOriginal, "12345");
        FileCompress(fileOriginal, fileCompressed);
        FileDecompress(fileCompressed, fileDecompressed);

        Assert.Equal("12345", File.ReadAllText(fileDecompressed));
    }

    private void FileCompress(string fileOriginal, string fileCompressed)
    {
        XZCompressOptions compOpts = new XZCompressOptions
        {
            Level = LzmaCompLevel.Default,
            ExtremeFlag = false,
        };

        using (FileStream fsOrigin = new FileStream(fileOriginal, FileMode.Open))
        using (FileStream fsComp = new FileStream(fileCompressed, FileMode.Create))
        using (XZStream zs = new XZStream(fsComp, compOpts))
        {
            fsOrigin.CopyTo(zs);
        }
    }

    private void FileDecompress(string fileCompressed, string fileDecompressed)
    {
        XZDecompressOptions decompOpts = new XZDecompressOptions();

        using (FileStream fsComp = new FileStream(fileCompressed, FileMode.Open))
        using (FileStream fsDecomp = new FileStream(fileDecompressed, FileMode.Create))
        using (XZStream zs = new XZStream(fsComp, decompOpts))
        {
            zs.CopyTo(fsDecomp);
        }
    }
}