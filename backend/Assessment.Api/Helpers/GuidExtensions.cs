
using System.Security.Cryptography;
using System.Text;

public static class GuidExtensions
{
    extension(Guid) {
        public static Guid NewGuid(string seed)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(seed);
            byte[] hashBytes = MD5.HashData(inputBytes);
            return new Guid(hashBytes);
        }
    }
}
