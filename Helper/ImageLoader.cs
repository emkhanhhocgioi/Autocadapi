using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace test.Helper
{
    public class ImageLoader
    {
        public BitmapImage LoadFromResource(string resourcePath)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            using (Stream stream = asm.GetManifestResourceStream(resourcePath))
            {
                if (stream == null) return null;

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze(); // BẮT BUỘC cho AutoCAD + WPF

                return image;
            }
        }

    }
}
