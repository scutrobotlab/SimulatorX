using System.IO;
using System.IO.Compression;
using System.Text;

namespace Misc
{
    public class GZipCompress
    {
        private static byte[] Compress(byte[] input)
        {
            var outStream = new MemoryStream();
            var zipStream = new GZipStream(outStream, CompressionMode.Compress);
            zipStream.Write(input, 0, input.Length);
            zipStream.Close();
            return outStream.ToArray();
        }

        private static byte[] Decompress(byte[] input)
        {
            var inStream = new MemoryStream(input);
            var outStream = new MemoryStream();
            var zipStream = new GZipStream(inStream, CompressionMode.Decompress);
            zipStream.CopyTo(outStream);
            zipStream.Close();
            return outStream.ToArray();
        }

        public static byte[] Compress(string input)
        {
            var inputBytes = Encoding.Default.GetBytes(input);
            var result = Compress(inputBytes);
            return result;
        }

        public static string DecompressToString(byte[] input)
        {
            var depressBytes = Decompress(input);
            return Encoding.Default.GetString(depressBytes);
        }
    }
}