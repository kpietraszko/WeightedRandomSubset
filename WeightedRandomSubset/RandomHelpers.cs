using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace WeightedRandomSubset;

public static class RandomHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Random01()
    {
        var randInt = RandomNumberGenerator.GetInt32(1, int.MaxValue);
        return (double)randInt / int.MaxValue;
    }
}
