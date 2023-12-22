using System.Text;
using JetBrains.Annotations;
using Unflow.ArticleBlobStorage;

namespace Unflow.Tests.ArticleBlobStorage;

[TestSubject(typeof(XZCompressorTests))]
public class XZCompressorTests
{
    [Fact]
    public async Task Compress_WithValidInput_ReturnsCompressedData()
    {
        // Arrange
        var compressor = new XZCompressor();
        var inputData = Encoding.UTF8.GetBytes("12345");

        // Act
        var result = await compressor.Compress(inputData);

        // Assert
        Assert.NotEqual(0, result.Length);
        Assert.NotEqual(inputData, result);
    }

    [Fact]
    public async Task Decompress_WithCompressedInput_ReturnsOriginalData()
    {
        // Arrange
        var compressor = new XZCompressor();
        var inputData = new byte[] { 1, 2, 3, 4, 5 };
        var compressedData = await compressor.Compress(inputData);

        // Act
        var result = await compressor.Decompress(compressedData);

        // Assert
        Assert.Equal(inputData, result);
    }
}