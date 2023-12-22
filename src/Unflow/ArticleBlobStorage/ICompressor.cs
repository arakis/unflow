namespace Unflow.ArticleBlobStorage
{
    public interface ICompressor
    {
        Task<byte[]> Compress(byte[] data);
        Task<byte[]> Decompress(byte[] data);
    }
}