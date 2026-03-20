using Avalonia.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace DialogueEditor.Avalonia.Utils
{
    static class ImageLoader
    {
        public static IBitmap CreateCroppedBitmapFromFile(string filePath)
        {
            using (var image = Image.Load(filePath))
            {
                image.Mutate(x => x.EntropyCrop());

                using (var memoryStream = new MemoryStream())
                {
                    var imageEncoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
                    image.Save(memoryStream, imageEncoder);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return new Bitmap(memoryStream);
                }
            }
        }
    }
}
