using System.Security.Cryptography;

public class SecureGuidGenerator
{
    private RNGCryptoServiceProvider _rng;
    private object _lock = new object();
    private readonly byte[] _bytes = new byte[16];

    public SecureGuidGenerator()
    {
        _rng = new RNGCryptoServiceProvider();
    }
    
    public Guid NewGuid()
    {
        Guid id;
        lock (_lock)
        {
            _rng.GetBytes(_bytes);
            id = new Guid(_bytes);
        }
        return id;
    }
    
    public static SecureGuidGenerator Default { get; } = new SecureGuidGenerator();
}