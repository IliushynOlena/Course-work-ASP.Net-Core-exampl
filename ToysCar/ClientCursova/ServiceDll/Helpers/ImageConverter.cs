using System;
using System.Drawing;
using System.IO;

namespace ServiceDll.Helpers
{
    public static class ImageConverter
    {
        public static string ConvertToBase64String(this Image image)
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();

                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }
}
