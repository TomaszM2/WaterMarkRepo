using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    class Program
    {
        private static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        private static void MergeImages(string path, string resultPath)
        {
            using (Image image = Image.FromFile(path))
            using (Image preaprewatermark = Image.FromFile(@"../../wzór.png"))
            using (Image watermarkImage = ScaleImage(preaprewatermark, image.Width, image.Height))
            using (Graphics imageGraphics = Graphics.FromImage(image))
            using (TextureBrush watermarkBrush = new TextureBrush(watermarkImage))
            {
                int x = (image.Width / 2 - watermarkImage.Width / 2);
                int y = (image.Height / 2 - watermarkImage.Height / 2);
                watermarkBrush.TranslateTransform(x, y);
                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));
                image.Save(resultPath);
            }
        }

        public static void GetFiles(string path, string resultpath)
        {
            var jpgs = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories).ToList();
            int counter = 1;
            foreach (var jpg in jpgs)
            {
                MergeImages(jpg, Path.Combine(resultpath, string.Concat(counter, @".jpg")));
                Console.WriteLine(string.Format("Przetworzone zdjęcia: {0} z {1}", counter, jpgs.Count()));
                counter++;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Podaj ścieżkę ze zdjęciami do tagowania");
            string path = Console.ReadLine();
            Console.WriteLine("Podaj ścieżkę do folderu wyjściowego");
            string resultPath = Console.ReadLine();

            try
            {
                GetFiles(path, resultPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Coś poszło nie tak!" + ex);
            }

            Console.WriteLine("Zdjęcia zostały przetworzone. Wciśnij dowolny przycisk");
            Console.ReadKey();

        }
    }
}
