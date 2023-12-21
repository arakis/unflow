using JetBrains.Annotations;
using Unflow.ArticleBlobStorage;

namespace Unflow.Tests.ArticleBlobStorage;

[TestSubject(typeof(HeaderEncoding))]
public class HeaderEncodingTest
{


    [Fact]
    public void Encode_WithKnownHeader_ReturnsEncodedHeader()
    {
        // Arrange
        var headerEncoding = new HeaderEncoding();
        var originalHeader = "From: John Doe";

        // Act
        var result = headerEncoding.Encode(originalHeader);

        // Assert
        Assert.Equal("f:John Doe", result);
    }

    [Fact]
    public void Encode_WithUnknownHeader_ReturnsOriginalHeader()
    {
        // Arrange
        var headerEncoding = new HeaderEncoding();
        var originalHeader = "Unknown: John Doe";

        // Act
        var result = headerEncoding.Encode(originalHeader);

        // Assert
        Assert.Equal("_:Unknown: John Doe", result);
    }

    [Fact]
    public void Decode_WithEncodedKnownHeader_ReturnsOriginalHeader()
    {
        // Arrange
        var headerEncoding = new HeaderEncoding();
        var encodedHeader = "f:John Doe";

        // Act
        var result = headerEncoding.Decode(encodedHeader);

        // Assert
        Assert.Equal("From: John Doe", result);
    }

    [Fact]
    public void Decode_WithEncodedUnknownHeader_ReturnsOriginalHeader()
    {
        // Arrange
        var headerEncoding = new HeaderEncoding();
        var encodedHeader = "_:Unknown: John Doe";

        // Act
        var result = headerEncoding.Decode(encodedHeader);

        // Assert
        Assert.Equal("Unknown: John Doe", result);
    }
    
}