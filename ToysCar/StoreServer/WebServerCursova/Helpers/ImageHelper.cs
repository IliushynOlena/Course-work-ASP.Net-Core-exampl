using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WebServerCursova.Helpers
{
    public static class ImageHelper
    {
        public static Bitmap FromBase64StringToImage(this string base64String)
        {
            try
            {
                byte[] byteBuffer = Convert.FromBase64String(base64String);

                using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
                {
                    memoryStream.Position = 0;
                    using (Image imgReturn = Image.FromStream(memoryStream))
                    {
                        memoryStream.Close();
                        byteBuffer = null;
                        return new Bitmap(imgReturn);
                    }
                }
            }
            catch { return null; }
        }

        /// <summary>
        /// Для стиску довільного фото у вказаному розмірі
        /// </summary>
        /// <param name="originalPic">Вхідна фотка</param>
        /// <param name="maxWidth">Максимальна ширина </param>
        /// <param name="maxHeight">Максимальна висота</param>
        /// <returns>Вертаю фотку уже готову, якщо щось пішло не так то буде NULL</returns>
        public static Bitmap CreateImage(Bitmap originalPic, int maxWidth, int maxHeight)
        {
            int width = originalPic.Width;
            int height = originalPic.Height;
            int widthDiff = width - maxWidth;
            int heightDiff = height - maxHeight;

            bool doWidthResize = (maxWidth > 0 && width > maxWidth && widthDiff > heightDiff);
            bool doHeightResize = (maxHeight > 0 && height > maxHeight && heightDiff > widthDiff);

            try
            {
                if (doWidthResize || doHeightResize || (width.Equals(height) && widthDiff.Equals(heightDiff)))
                {
                    int iStart;
                    Decimal divider;
                    if (doWidthResize)
                    {
                        iStart = width;
                        divider = Math.Abs((Decimal)iStart / maxWidth);
                        width = maxWidth;
                        height = (int)Math.Round((height / divider));
                    }
                    else
                    {
                        iStart = height;
                        divider = Math.Abs((Decimal)iStart / maxHeight);
                        height = maxHeight;
                        width = (int)Math.Round(width / divider);
                    }
                }
                using (Bitmap outBmp = new Bitmap(width, height, PixelFormat.Format24bppRgb))
                {
                    using (Graphics oGraphics = Graphics.FromImage(outBmp))
                    {
                        oGraphics.DrawImage(originalPic, 0, 0, width, height);
                        return new Bitmap(outBmp);
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        public static string GetImageFolder(IHostingEnvironment env, IConfiguration configuration)
        {
            var rootPath = env.ContentRootPath; // шлях до кореневої папки
            string dirName = configuration.GetValue<string>("ImagesPath");  //папка, де зберігатимуться фото
            string dirPathSave = Path.Combine(rootPath, dirName);

            if (!Directory.Exists(dirPathSave))
            {
                Directory.CreateDirectory(dirPathSave);
            }

            return dirPathSave;
        }
    }
}
