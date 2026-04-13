using System.Security.Cryptography;
using System.Text;

namespace Halyr.Api.Helpers;

public static class HashingHelper
{
    public static int GetBucket(string flagKey, Guid userId)
    {
        var input = $"{flagKey}:{userId}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var value = BitConverter.ToUInt32(bytes, 0);

        return (int)(value % 100);
    }
}