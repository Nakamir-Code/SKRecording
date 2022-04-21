using Windows.Graphics.Imaging;
using System.IO;

namespace SKRecording
{
    public class ImageParser
    {
        public static BitmapFrame decompress(byte[][] compressedFragments, bool isJpeg)
        {
            byte[] compressedImg = Utils.concatBytes(compressedFragments);
            if (isJpeg)
            {
                MemoryStream memStream = new MemoryStream(compressedImg);
                BitmapDecoder decoder = BitmapDecoder.CreateAsync(memStream.AsRandomAccessStream()).GetResults();
                return decoder.GetFrameAsync(0).GetResults();
            }
            // RVL
            else
            {
                // TODO: Implement RVL dll
                throw new System.Exception("Not implemented yet");
            }
        }

    }
}
