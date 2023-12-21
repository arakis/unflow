using System.Text;
using JetBrains.Annotations;
using Unflow.ArticleBlobStorage;

namespace Unflow.Tests.ArticleBlobStorage;

[TestSubject(typeof(XZCompressorTests))]
public class XZCompressorTests
{
    [Fact]
    public void Compress_WithValidInput_ReturnsCompressedData()
    {
        // Arrange
        var compressor = new XZCompressor();
        var inputData = Encoding.UTF8.GetBytes("12345");

        // Act
        var result = compressor.Compress(inputData);

        // Assert
        Assert.NotEqual(0, result.Length);
        Assert.NotEqual(inputData, result);
    }

    [Fact]
    public void Decompress_WithCompressedInput_ReturnsOriginalData()
    {
        // Arrange
        var compressor = new XZCompressor();
        var inputData = new byte[] { 1, 2, 3, 4, 5 };
        var compressedData = compressor.Compress(inputData);

        // Act
        var result = compressor.Decompress(compressedData);

        // Assert
        Assert.Equal(inputData, result);
    }
}