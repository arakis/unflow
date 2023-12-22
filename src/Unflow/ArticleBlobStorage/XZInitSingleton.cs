using System.Runtime.CompilerServices;
using Joveler.Compression.XZ;

namespace Unflow.ArticleBlobStorage;

/// <summary>
/// This class is used to force static constructor of XZLib to run.
/// It garantees that XZLib is only initialized once.
/// </summary>
public static class XZInitSingleton
{
    static XZInitSingleton()
    {
        XZInit.GlobalInit();
    }

    /// <summary>
    /// Dummy method to force static constructor to run
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void Init()
    {
    }
}