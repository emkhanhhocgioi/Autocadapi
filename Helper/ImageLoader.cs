using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace test.Helper
{
    public class ImageLoader
    {
        public BitmapImage LoadFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    System.Windows.Forms.MessageBox.Show($"Image not found: {filePath}");
                    return null;
                }

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(filePath, UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Important for cross-thread access

                return bitmapImage;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error loading image: {ex.Message}");
                return null;
            }
        }
    }
}