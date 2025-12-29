using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;

namespace FF7Scarlet.Tests
{
    public class ImageTestBase
    {
        protected static string GetImageHash(Image image)
        {
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            var hash = SHA256.HashData(ms.ToArray());
            return Convert.ToHexString(hash)[..8];
        }
    }
}
